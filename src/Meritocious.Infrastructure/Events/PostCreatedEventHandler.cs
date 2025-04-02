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
    using Microsoft.Extensions.Logging;

    public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
    {
        private readonly IMeritScoringService _meritScoringService;
        private readonly ContentSimilarityRepository _similarityRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostCreatedEventHandler> _logger;

        public PostCreatedEventHandler(
            IMeritScoringService meritScoringService,
            ContentSimilarityRepository similarityRepository,
            IPostRepository postRepository,
            ILogger<PostCreatedEventHandler> logger)
        {
            _meritScoringService = meritScoringService;
            _similarityRepository = similarityRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Recalculate user's merit score
                await _meritScoringService.CalculateUserMeritScoreAsync(notification.AuthorId);

                // Get recently active posts for initial similarity comparison
                var recentPosts = await _postRepository.Query
                    .Where(p => p.CreatedAt > DateTime.UtcNow.AddDays(-30))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                // Add the new post ID
                recentPosts.Add(notification.PostId);

                // Create similarity records for the new post with recent posts
                await _similarityRepository.CreateMissingSimilaritiesAsync(recentPosts);

                // Mark these new records for priority update
                await _similarityRepository.MarkForUpdateAsync(notification.PostId, priority: 2);

                _logger.LogInformation(
                    "Handled PostCreatedEvent for Post {PostId} by User {UserId}. Created {Count} similarity records.",
                    notification.PostId,
                    notification.AuthorId,
                    recentPosts.Count - 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling PostCreatedEvent for Post {PostId}",
                    notification.PostId);
            }
        }
    }
}