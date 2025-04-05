using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.Core.Events;
    using Meritocious.Infrastructure.Data.Resolvers;

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<Post>>
    {
        private readonly PostRepository postRepository;
        private readonly IMeritScoreTypeResolver meritScoreTypeResolver;
        private readonly ITagService tagService;
        private readonly IMeritScorer meritScorer;
        private readonly IMediator mediator;

        public UpdatePostCommandHandler(
            PostRepository postRepository,
            IMeritScoreTypeResolver meritScoreTypeResolver,
            ITagService tagService,
            IMeritScorer meritScorer,
            IMediator mediator)
        {
            this.postRepository = postRepository;
            this.meritScoreTypeResolver = meritScoreTypeResolver;
            this.tagService = tagService;
            this.meritScorer = meritScorer;
            this.mediator = mediator;
        }

        public async Task<Result<Post>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                return Result.Failure<Post>($"Post {request.PostId} not found");
            }

            if (post.AuthorId != request.EditorId.ToString())
            {
                return Result.Failure<Post>("Only the author can edit this post");
            }

            // Validate content quality
            var contentScore = await meritScorer.ScoreContentAsync(request.Content);
            if (!await meritScorer.ValidateContentAsync(request.Content))
            {
                return Result.Failure<Post>("Content does not meet quality standards");
            }

            // Update post
            post.UpdateContent(request.Title, request.Content);
            post.UpdateMeritScore(contentScore, this.meritScoreTypeResolver.GetByName);

            // Update tags
            foreach (var tagNameAndCategory in request.Tags.Zip(request.TagCategories))
            {
                await tagService.AddTagToPostAsync(post.Id, tagNameAndCategory.First, tagNameAndCategory.Second);
            }

            await postRepository.UpdateAsync(post);

            // Publish event
            await mediator.Publish(new PostUpdatedEvent(post.Id, request.EditorId), cancellationToken);

            return Result.Success(post);
        }
    }
}