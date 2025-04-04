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
        private readonly IMeritScoringService meritScoringService;
        private readonly IPostService postService;
        private readonly ILogger<CommentAddedEventHandler> logger;

        public CommentAddedEventHandler(
            IMeritScoringService meritScoringService,
            IPostService postService,
            ILogger<CommentAddedEventHandler> logger)
        {
            this.meritScoringService = meritScoringService;
            this.postService = postService;
            this.logger = logger;
        }

        public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Recalculate user's merit score
                await meritScoringService.CalculateUserMeritScoreAsync(notification.AuthorId);

                // Update post's activity score
                await postService.UpdatePostActivityAsync(notification.PostId);

                logger.LogInformation(
                    "Processed CommentAddedEvent for Comment {CommentId} on Post {PostId}",
                    notification.CommentId,
                    notification.PostId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error processing CommentAddedEvent for Comment {CommentId}",
                    notification.CommentId);
            }
        }
    }
}