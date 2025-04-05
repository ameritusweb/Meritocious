namespace Meritocious.Core.Features.Reporting.Events
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ContentReportedEventHandler : INotificationHandler<ContentReportedEvent>
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;
        private readonly ILogger<ContentReportedEventHandler> logger;

        public ContentReportedEventHandler(
            INotificationService notificationService,
            IUserService userService,
            ILogger<ContentReportedEventHandler> logger)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task Handle(ContentReportedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Get list of moderators
                var moderators = await userService.GetModeratorsAsync();

                // Notify each moderator
                foreach (var moderator in moderators)
                {
                    await notificationService.SendNotificationAsync(
                        Notification.Create(
                            moderator.Id,
                            "ContentReport",
                            "New Content Report",
                            $"Content reported for {notification.Report.ReportType}: {notification.Report.Description}",
                            $"/moderation/reports/{notification.Report.Id}"));
                }

                logger.LogInformation(
                    "Notified moderators about content report {ReportId}",
                    notification.Report.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error handling ContentReportedEvent for report {ReportId}",
                    notification.Report.Id);
            }
        }
    }
}
