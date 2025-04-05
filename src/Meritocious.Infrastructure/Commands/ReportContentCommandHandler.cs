using MediatR;
using Meritocious.Core.Features.Moderation.Commands;
using Meritocious.Core.Features.Reporting.Commands;
using Meritocious.Core.Features.Reporting.Events;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class ReportContentCommandHandler : IRequestHandler<ReportContentCommand, Result>
    {
        private readonly IReportingService reportingService;
        private readonly IMediator mediator;
        private readonly ILogger<ReportContentCommandHandler> logger;

        public ReportContentCommandHandler(
            IReportingService reportingService,
            IMediator mediator,
            ILogger<ReportContentCommandHandler> logger)
        {
            this.reportingService = reportingService;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<Result> Handle(ReportContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await reportingService.CreateReportAsync(
                    request.ContentId,
                    request.ContentType,
                    request.ReporterId,
                    request.ReportType,
                    request.Description);

                // Trigger automatic moderation if needed
                if (await reportingService.ShouldTriggerModerationAsync(report))
                {
                    await mediator.Send(new ModerateContentCommand
                    {
                        ContentId = request.ContentId,
                        ContentType = request.ContentType,
                        IsAutomated = true
                    });
                }

                // Notify moderators
                await mediator.Publish(new ContentReportedEvent(report));

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing content report");
                return Result.Failure(ex.Message);
            }
        }
    }
}
