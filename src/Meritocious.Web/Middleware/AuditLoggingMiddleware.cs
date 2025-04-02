using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Meritocious.Web.Middleware
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuditLogAttribute>() != null)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var action = context.Request.Path;
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                _logger.LogInformation(
                    "Audit: {Action} by User {UserId} from {IpAddress} using {UserAgent}",
                    action, userId, ipAddress, userAgent);

                // Store audit record
                var audit = new AdminActionLog
                {
                    AdminUserId = Guid.Parse(userId ?? string.Empty),
                    Action = action,
                    IpAddress = ipAddress ?? "unknown",
                    Timestamp = DateTime.UtcNow,
                    Details = $"User-Agent: {userAgent}"
                };

                var auditService = context.RequestServices.GetRequiredService<ISecurityAuditService>();
                await auditService.LogAdminActionAsync(audit);
            }

            await _next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuditLogAttribute : Attribute
    {
    }
}