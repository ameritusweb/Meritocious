using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Events
{
    using MediatR;
    using Meritocious.Core.Features.Notifications.Models;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ContentReportedEventHandler : INotificationHandler<ContentReportedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ILogger<ContentReportedEventHandler> _logger;

        public ContentReportedEventHandler(
            INotificationService notificationService,
            IUserService userService,
            ILogger<ContentReportedEventHandler> logger)
        {
            _notificationService = notificationService;
            _userService = userService;
            _logger = logger;
        }

        public async Task Handle(ContentReportedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Get list of moderators
                var moderators = await _userService.GetModeratorsAsync();

                // Notify each moderator
                foreach (var moderator in moderators)
                {
                    await _notificationService.SendNotificationAsync(
                        Notification.Create(
                            moderator.Id,
                            "ContentReport",
                            "New Content Report",
                            $"Content reported for {notification.Report.ReportType}: {notification.Report.Description}",
                            $"/moderation/reports/{notification.Report.Id}"
                        ));
                }

                _logger.LogInformation(
                    "Notified moderators about content report {ReportId}",
                    notification.Report.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling ContentReportedEvent for report {ReportId}",
                    notification.Report.Id);
            }
        }
    }
}
