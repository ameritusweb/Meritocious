using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Events
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class ReportResolvedEventHandler : INotificationHandler<ReportResolvedEvent>
    {
        private readonly INotificationService notificationService;
        private readonly ILogger<ReportResolvedEventHandler> logger;

        public ReportResolvedEventHandler(
            INotificationService notificationService,
            ILogger<ReportResolvedEventHandler> logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        public async Task Handle(ReportResolvedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Notify the reporter
                await notificationService.SendNotificationAsync(
                    Notification.Create(
                        notification.Report.ReporterId.ToString(),
                        "ReportResolved",
                        "Your Report Has Been Resolved",
                        $"A moderator has reviewed and resolved your report: {notification.Resolution}",
                        $"/reports/{notification.Report.Id}"));

                logger.LogInformation(
                    "Notified user {UserId} about resolved report {ReportId}",
                    notification.Report.ReporterId,
                    notification.Report.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error handling ReportResolvedEvent for report {ReportId}",
                    notification.Report.Id);
            }
        }
    }
}