using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Infrastructure.Data;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;

namespace Meritocious.Infrastructure.Queries.Security
{
    public class GetSecurityOverviewQueryHandler : IRequestHandler<GetSecurityOverviewQuery, SecurityOverviewDto>
    {
        private readonly MeritociousDbContext context;
        private readonly ILogger<GetSecurityOverviewQueryHandler> logger;

        public GetSecurityOverviewQueryHandler(
            MeritociousDbContext context,
            ILogger<GetSecurityOverviewQueryHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<SecurityOverviewDto> Handle(
            GetSecurityOverviewQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var oneDayAgo = DateTime.UtcNow.AddDays(-1);

                // Get failed login attempts in last 24h
                var failedLogins = await context.LoginAttempts
                    .Where(l => !l.Success && l.Timestamp >= oneDayAgo)
                    .ToListAsync(cancellationToken);

                // Get unique suspicious IPs
                var suspiciousIps = await context.SecurityAuditLogs
                    .Where(l => l.Timestamp >= oneDayAgo && l.Severity != "low")
                    .Select(l => l.IpAddress)
                    .Distinct()
                    .CountAsync(cancellationToken);

                // Get security incidents
                var securityIncidents = await context.SecurityAuditLogs
                    .Where(l => l.Timestamp >= oneDayAgo && l.Severity == "high")
                    .CountAsync(cancellationToken);

                // Get API request count
                var apiRequests = await context.ApiUsageLogs
                    .Where(l => l.Timestamp >= oneDayAgo)
                    .CountAsync(cancellationToken);

                // Get recent failed logins
                var recentFailedLogins = await context.LoginAttempts
                    .Where(l => !l.Success)
                    .OrderByDescending(l => l.Timestamp)
                    .Take(5)
                    .Select(l => new LoginAttemptDto
                    {
                        Id = l.Id.ToString(),
                        Username = l.Username,
                        Success = l.Success,
                        IpAddress = l.IpAddress,
                        UserAgent = l.UserAgent,
                        FailureReason = l.FailureReason,
                        Timestamp = l.Timestamp,
                        Location = l.Location,
                        Device = l.Device
                    })
                    .ToListAsync(cancellationToken);

                // Get recent security events
                var recentSecurityEvents = await context.SecurityAuditLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(5)
                    .Select(l => new SecurityAuditLogDto
                    {
                        Id = l.Id,
                        EventType = l.EventType,
                        Severity = l.Severity,
                        Description = l.Description,
                        IpAddress = l.IpAddress,
                        Username = l.User.UserName,
                        UserAgent = l.UserAgent,
                        Timestamp = l.Timestamp,
                        Context = l.Context
                    })
                    .ToListAsync(cancellationToken);

                // Get API endpoint usage stats
                var endpointUsage = await context.ApiUsageLogs
                    .Where(l => l.Timestamp >= oneDayAgo)
                    .GroupBy(l => l.Endpoint)
                    .Select(g => new { Endpoint = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(
                        x => x.Endpoint,
                        x => x.Count,
                        cancellationToken);

                return new SecurityOverviewDto
                {
                    FailedLoginAttempts24h = failedLogins.Count,
                    UniqueSuspiciousIps24h = suspiciousIps,
                    SecurityIncidents24h = securityIncidents,
                    TotalApiRequests24h = apiRequests,
                    RecentFailedLogins = recentFailedLogins,
                    RecentSecurityEvents = recentSecurityEvents,
                    ApiEndpointUsage = endpointUsage
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting security overview");
                throw;
            }
        }
    }
}