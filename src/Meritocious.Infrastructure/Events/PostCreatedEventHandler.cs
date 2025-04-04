using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.EventHandlers
{
    using MediatR;
    using Meritocious.Core.Events;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Microsoft.Extensions.Logging;

    public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
    {
        private readonly IMeritScoringService meritScoringService;
        private readonly ContentSimilarityRepository similarityRepository;
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostCreatedEventHandler> logger;

        public PostCreatedEventHandler(
            IMeritScoringService meritScoringService,
            ContentSimilarityRepository similarityRepository,
            IPostRepository postRepository,
            ILogger<PostCreatedEventHandler> logger)
        {
            this.meritScoringService = meritScoringService;
            this.similarityRepository = similarityRepository;
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Recalculate user's merit score
                await meritScoringService.CalculateUserMeritScoreAsync(notification.AuthorId);

                // Get recently active posts for initial similarity comparison
                var recentPosts = new List<Guid>();
                
                // TODO: Get recent posts.
                // await postRepository.Query
                //    .Where(p => p.CreatedAt > DateTime.UtcNow.AddDays(-30))
                //    .Select(p => p.Id)
                //    .ToListAsync(cancellationToken);

                // Add the new post ID
                recentPosts.Add(notification.PostId);

                // Create similarity records for the new post with recent posts
                await similarityRepository.CreateMissingSimilaritiesAsync(recentPosts);

                // Mark these new records for priority update
                await similarityRepository.MarkForUpdateAsync(notification.PostId, priority: 2);

                logger.LogInformation(
                    "Handled PostCreatedEvent for Post {PostId} by User {UserId}. Created {Count} similarity records.",
                    notification.PostId,
                    notification.AuthorId,
                    recentPosts.Count() - 1);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error handling PostCreatedEvent for Post {PostId}",
                    notification.PostId);
            }
        }
    }
}