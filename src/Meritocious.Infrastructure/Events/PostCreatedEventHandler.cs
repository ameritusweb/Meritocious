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
        private readonly ILogger<PostCreatedEventHandler> _logger;

        public PostCreatedEventHandler(
            IMeritScoringService meritScoringService,
            ILogger<PostCreatedEventHandler> logger)
        {
            _meritScoringService = meritScoringService;
            _logger = logger;
        }

        public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Recalculate user's merit score
                await _meritScoringService.CalculateUserMeritScoreAsync(notification.AuthorId);

                _logger.LogInformation(
                    "Handled PostCreatedEvent for Post {PostId} by User {UserId}",
                    notification.PostId,
                    notification.AuthorId);
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