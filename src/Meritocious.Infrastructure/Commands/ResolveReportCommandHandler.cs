using MediatR;
using Meritocious.Common.DTOs.Moderation;
using Meritocious.Common.Enums;
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
    public class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand, Result>
    {
        private readonly IReportingService _reportingService;
        private readonly IMediator _mediator;
        private readonly ILogger<ResolveReportCommandHandler> _logger;

        public ResolveReportCommandHandler(
            IReportingService reportingService,
            IMediator mediator,
            ILogger<ResolveReportCommandHandler> logger)
        {
            _reportingService = reportingService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _reportingService.GetReportByIdAsync(request.ReportId);
                if (report == null)
                    return Result.Failure($"Report {request.ReportId} not found");

                // Update report status
                await _reportingService.ResolveReportAsync(
                    request.ReportId,
                    request.ModeratorId,
                    request.Resolution,
                    request.Notes);

                // Apply moderation action if needed
                if (request.Action.ActionType != ModerationActionType.None)
                {
                    await _mediator.Send(new ModerateContentCommand
                    {
                        ContentId = report.ContentId,
                        ContentType = report.ContentType,
                        IsAutomated = false,
                        ModeratorId = request.ModeratorId
                    });
                }

                // Notify reporter of resolution
                await _mediator.Publish(new ReportResolvedEvent(report, request.ModeratorId, request.Resolution));

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving report {ReportId}", request.ReportId);
                return Result.Failure(ex.Message);
            }
        }
    }
}
