namespace Meritocious.Core.Features.Search.Queries
{
    using MediatR;
    using Meritocious.AI.Clustering.Interfaces;
    using Meritocious.Core.Features.Discovery.Queries;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetTrendingTopicsQueryHandler : IRequestHandler<GetTrendingTopicsQuery, List<TrendingTopicDto>>
    {
        private readonly ITagService tagService;
        private readonly PostRepository postRepository;
        private readonly ISemanticClusteringService semanticClusteringService;

        public GetTrendingTopicsQueryHandler(
            ITagService tagService,
            PostRepository postRepository,
            ISemanticClusteringService semanticClusteringService)
        {
            this.tagService = tagService;
            this.postRepository = postRepository;
            this.semanticClusteringService = semanticClusteringService;
        }

        public async Task<List<TrendingTopicDto>> Handle(
            GetTrendingTopicsQuery request,
            CancellationToken cancellationToken)
        {
            var startTime = request.TimeFrame switch
            {
                "hour" => DateTime.UtcNow.AddHours(-1),
                "day" => DateTime.UtcNow.AddDays(-1),
                "week" => DateTime.UtcNow.AddDays(-7),
                "month" => DateTime.UtcNow.AddMonths(-1),
                _ => DateTime.UtcNow.AddDays(-1)
            };

            // Get recent posts
            var recentPosts = await postRepository.GetPostsAfterDateAsync(startTime);

            // Use semantic clustering to identify trending topics
            var topics = await semanticClusteringService.IdentifyTopicsAsync(
                recentPosts.Select(p => p.Content).ToList());

            var trendingTopics = new List<TrendingTopicDto>();
            foreach (var topic in topics)
            {
                var topicPosts = await postRepository.GetPostsByTopicAsync(topic, startTime);
                var relatedTags = await tagService.GetRelatedTagsAsync(topic);

                trendingTopics.Add(new TrendingTopicDto
                {
                    Topic = topic,
                    PostCount = topicPosts.Count,
                    CommentCount = topicPosts.Sum(p => p.Comments.Count),
                    AverageMeritScore = topicPosts.Average(p => p.MeritScore),
                    RelatedTags = relatedTags.Select(t => t.Name).ToList(),
                    TopPosts = topicPosts
                        .OrderByDescending(p => p.MeritScore)
                        .Take(3)
                        .Select(p => new PostSummaryDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            AuthorUsername = p.Author.Username,
                            MeritScore = p.MeritScore,
                            CreatedAt = p.CreatedAt
                        })
                        .ToList()
                });
            }

            return trendingTopics
                .OrderByDescending(t => t.PostCount + t.CommentCount)
                .Take(request.Count)
                .ToList();
        }
    }
}