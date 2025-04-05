using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Core.Features.Moderation.Queries;
using MediatR;
using Meritocious.Core.Results;

namespace Meritocious.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/admin")]
    public class AdminController : ApiControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IMediator _mediator;

        public AdminController(ILogger<AdminController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<AdminDashboardStatistics>> GetDashboardStats()
        {
            try
            {
                var statsTask = _mediator.Send(new GetAdminStatsQuery());
                var moderationStatsTask = _mediator.Send(new GetModerationStatsQuery());
                
                await Task.WhenAll(statsTask, moderationStatsTask);
                var stats = await statsTask;
                var modStats = await moderationStatsTask;
                
                return Ok(new AdminDashboardStatistics
                {
                    TotalUsers = stats.TotalUsers,
                    ActiveUsers24h = stats.ActiveUsers24h,
                    PostsToday = stats.PostsToday,
                    ActiveModerations = modStats.ActiveModerations,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard stats");
                return StatusCode(500, "Error retrieving dashboard statistics");
            }
        }

        [HttpGet("logs")]
        public async Task<ActionResult<PagedResult<LogEntryDto>>> GetSystemLogs([FromQuery] LogQueryParams queryParams)
        {
            try
            {
                var query = new GetSystemLogsQuery(
                    queryParams.Page,
                    queryParams.PageSize,
                    queryParams.Level,
                    queryParams.SearchText,
                    queryParams.StartDate,
                    queryParams.EndDate
                );

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system logs");
                return StatusCode(500, "Error retrieving system logs");
            }
        }

        [HttpGet("logs/recent")]
        public async Task<ActionResult<List<LogEntryDto>>> GetRecentLogs([FromQuery] int count = 5)
        {
            try
            {
                var query = new GetRecentLogsQuery(count);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent logs");
                return StatusCode(500, "Error retrieving recent logs");
            }
        }

        [HttpGet("logs/export")]
        public async Task<ActionResult<string>> GetLogExportUrl(
            [FromQuery] string? level,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? searchText)
        {
            try
            {
                var query = new GetLogExportUrlQuery(level, startDate, endDate, searchText);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating log export URL");
                return StatusCode(500, "Error generating export URL");
            }
        }
    }
}