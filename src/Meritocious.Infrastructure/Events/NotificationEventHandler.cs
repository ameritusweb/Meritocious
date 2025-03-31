using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Events
{
    using MediatR;
    using Meritocious.Core.Features.Notifications.Models;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class NotificationEventHandler : INotificationHandler<NotificationEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationEventHandler> _logger;

        public NotificationEventHandler(
            INotificationService notificationService,
            ILogger<NotificationEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(NotificationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var notif = Notification.Create(
                    notification.UserId,
                    notification.Type,
                    notification.Title,
                    notification.Message,
                    notification.ActionUrl);

                await _notificationService.SendNotificationAsync(notif);

                _logger.LogInformation(
                    "Sent notification to user {UserId} of type {Type}",
                    notification.UserId,
                    notification.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending notification to user {UserId}",
                    notification.UserId);
            }
        }
    }
}