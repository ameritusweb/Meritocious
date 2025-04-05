using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Features.Security.Queries;
using MediatR;
using Meritocious.Core.Results;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/security")]
    public class SecurityAuditController : ApiControllerBase
    {
        private readonly ILogger<SecurityAuditController> _logger;
        private readonly IMediator _mediator;

        public SecurityAuditController(ILogger<SecurityAuditController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<SecurityOverviewDto>> GetSecurityOverview()
        {
            try
            {
                var overview = await _mediator.Send(new GetSecurityOverviewQuery());
                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security overview");
                return StatusCode(500, "Error retrieving security overview");
            }
        }

        [HttpGet("admin-actions")]
        public async Task<ActionResult<PagedResult<AdminActionLogDto>>> GetAdminActions([FromQuery] AuditLogQueryParams queryParams)
        {
            try
            {
                var query = new GetAdminActionsQuery(queryParams.StartDate, queryParams.EndDate, queryParams.Page, queryParams.PageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin actions");
                return StatusCode(500, "Error retrieving admin actions");
            }
        }

        [HttpGet("security-events")]
        public async Task<ActionResult<PagedResult<SecurityAuditLogDto>>> GetSecurityEvents([FromQuery] AuditLogQueryParams queryParams)
        {
            try
            {
                var query = new GetSecurityEventsQuery(queryParams.StartDate, queryParams.EndDate, queryParams.Page, queryParams.PageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security events");
                return StatusCode(500, "Error retrieving security events");
            }
        }

        [HttpGet("login-attempts")]
        public async Task<ActionResult<PagedResult<LoginAttemptDto>>> GetLoginAttempts([FromQuery] AuditLogQueryParams queryParams)
        {
            try
            {
                var query = new GetLoginAttemptsQuery(queryParams.StartDate, queryParams.EndDate, queryParams.Page, queryParams.PageSize);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting login attempts");
                return StatusCode(500, "Error retrieving login attempts");
            }
        }

        [HttpGet("api-usage")]
        public async Task<ActionResult<PagedResult<ApiUsageLogDto>>> GetApiUsage([FromQuery] AuditLogQueryParams queryParams)
        {
            try
            {
                var query = new GetApiUsageQuery(queryParams.StartDate, queryParams.EndDate);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting API usage logs");
                return StatusCode(500, "Error retrieving API usage logs");
            }
        }

        [HttpGet("export")]
        public async Task<ActionResult> ExportAuditLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? category)
        {
            try
            {
                var query = new GetAuditLogExportQuery(startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), category);
                var fileBytes = await _mediator.Send(query);
                
                var fileName = $"audit-logs-{DateTime.UtcNow:yyyyMMdd}.csv";
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting audit logs");
                return StatusCode(500, "Error generating audit log export");
            }
        }
    }
}