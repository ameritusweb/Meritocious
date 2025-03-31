using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Events
{
    using MediatR;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CommentAddedEventHandler : INotificationHandler<CommentAddedEvent>
    {
        private readonly IMeritScoringService _meritScoringService;
        private readonly IPostService _postService;
        private readonly ILogger<CommentAddedEventHandler> _logger;

        public CommentAddedEventHandler(
            IMeritScoringService meritScoringService,
            IPostService postService,
            ILogger<CommentAddedEventHandler> logger)
        {
            _meritScoringService = meritScoringService;
            _postService = postService;
            _logger = logger;
        }

        public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Recalculate user's merit score
                await _meritScoringService.CalculateUserMeritScoreAsync(notification.AuthorId);

                // Update post's activity score
                await _postService.UpdatePostActivityAsync(notification.PostId);

                _logger.LogInformation(
                    "Processed CommentAddedEvent for Comment {CommentId} on Post {PostId}",
                    notification.CommentId,
                    notification.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing CommentAddedEvent for Comment {CommentId}",
                    notification.CommentId);
            }
        }
    }
}