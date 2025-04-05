using MediatR;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Core.Features.Moderation.Commands;
using Meritocious.Core.Features.Reporting.Commands;
using Meritocious.Core.Features.Reporting.Queries;
using Meritocious.Common.Enums;
using Meritocious.Core.Features.Merit.Commands;
using Meritocious.Core.Results;
using Meritocious.Core.Entities;

namespace Meritocious.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModerationController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ModerationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("reports")]
    public async Task<ActionResult> ReportContent(ReportContentCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("reports")]
    public async Task<ActionResult<List<ContentReport>>> GetReports(
        [FromQuery] string status = "pending",
        [FromQuery] string sortBy = "date",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetContentReportsQuery
        {
            Status = status,
            SortBy = sortBy,
            Page = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("reports/{id}/resolve")]
    public async Task<ActionResult> ResolveReport(Guid id, ResolveReportCommand command)
    {
        if (id != command.ReportId)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("moderate")]
    public async Task<ActionResult<ModerationResult>> ModerateContent(ModerateContentCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("recalculate-merit")]
    public async Task<ActionResult<decimal>> RecalculateMeritScore(RecalculateMeritScoreCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("reports/stats")]
    public async Task<ActionResult<ModerationStatsDto>> GetModerationStats(
        [FromQuery] string timeFrame = "day")
    {
        var query = new GetModerationStatsQuery { TimeFrame = timeFrame };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("content/{id}/history")]
    public async Task<ActionResult<List<ModerationHistoryDto>>> GetModerationHistory(
        Guid id,
        [FromQuery] ContentType contentType)
    {
        var query = new GetModerationHistoryQuery
        {
            ContentId = id,
            ContentType = contentType
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}

public record GetModerationStatsQuery : IRequest<ModerationStatsDto>
{
    public string TimeFrame { get; init; }
}

public class ModerationStatsDto
{
    public int TotalReports { get; set; }
    public int PendingReports { get; set; }
    public int ResolvedReports { get; set; }
    public Dictionary<string, int> ReportsByType { get; set; } = new();
    public Dictionary<string, int> ActionsTaken { get; set; } = new();
    public decimal AverageResolutionTime { get; set; }
    public int ActiveModerations { get; set; }
}

public record GetModerationHistoryQuery : IRequest<Result<List<ModerationHistoryDto>>>
{
    public Guid ContentId { get; init; }
    public ContentType ContentType { get; init; }
}

public class ModerationHistoryDto
{
    public DateTime Timestamp { get; set; }
    public ModerationAction Action { get; set; }
    public string Reason { get; set; }
    public bool IsAutomated { get; set; }
    public string ModeratorUsername { get; set; }
    public decimal? MeritScoreBefore { get; set; }
    public decimal? MeritScoreAfter { get; set; }
}