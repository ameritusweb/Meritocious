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

    public class ReportResolvedEventHandler : INotificationHandler<ReportResolvedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReportResolvedEventHandler> _logger;

        public ReportResolvedEventHandler(
            INotificationService notificationService,
            ILogger<ReportResolvedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(ReportResolvedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Notify the reporter
                await _notificationService.SendNotificationAsync(
                    Notification.Create(
                        notification.Report.ReporterId,
                        "ReportResolved",
                        "Your Report Has Been Resolved",
                        $"A moderator has reviewed and resolved your report: {notification.Resolution}",
                        $"/reports/{notification.Report.Id}"
                    ));

                _logger.LogInformation(
                    "Notified user {UserId} about resolved report {ReportId}",
                    notification.Report.ReporterId,
                    notification.Report.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling ReportResolvedEvent for report {ReportId}",
                    notification.Report.Id);
            }
        }
    }
}