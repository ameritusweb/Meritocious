namespace Meritocious.Core.Features.Notifications.Events
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class NotificationEventHandler : INotificationHandler<NotificationEvent>
    {
        private readonly INotificationService notificationService;
        private readonly ILogger<NotificationEventHandler> logger;

        public NotificationEventHandler(
            INotificationService notificationService,
            ILogger<NotificationEventHandler> logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        public async Task Handle(NotificationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var notif = Notification.Create(
                    notification.UserId.ToString(),
                    notification.Type,
                    notification.Title,
                    notification.Message,
                    notification.ActionUrl);

                await notificationService.SendNotificationAsync(notif);

                logger.LogInformation(
                    "Sent notification to user {UserId} of type {Type}",
                    notification.UserId,
                    notification.Type);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error sending notification to user {UserId}",
                    notification.UserId);
            }
        }
    }
}