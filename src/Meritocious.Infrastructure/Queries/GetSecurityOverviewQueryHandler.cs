using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetSecurityOverviewQueryHandler : IRequestHandler<GetSecurityOverviewQuery, SecurityOverviewDto>
{
    private readonly MeritociousDbContext context;

    public GetSecurityOverviewQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<SecurityOverviewDto> Handle(GetSecurityOverviewQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var last24Hours = now.AddHours(-24);

        var totalUsers = await context.Users.CountAsync(cancellationToken);
        var activeUsers = await context.Users.CountAsync(u => u.LastActivityAt >= last24Hours, cancellationToken);
        var lockedAccounts = await context.Users.CountAsync(u => u.IsLocked, cancellationToken);
        var unverifiedAccounts = await context.Users.CountAsync(u => !u.IsEmailVerified, cancellationToken);

        var failedLogins = await context.LoginAttempts
            .CountAsync(l => !l.Success && l.Timestamp >= last24Hours, cancellationToken);

        var securityIncidents = await context.SecurityEvents
            .CountAsync(e => e.Timestamp >= last24Hours, cancellationToken);

        var pendingModerationActions = await context.ModerationActions
            .CountAsync(m => m.Status == "Pending", cancellationToken);

        var blockedIps = await context.BlockedIpAddresses
            .CountAsync(b => b.ExpiresAt > now, cancellationToken);

        var activeSessions = await context.UserSessions
            .CountAsync(s => s.LastActivityAt >= last24Hours, cancellationToken);

        var apiRequests = await context.ApiUsageLogs
            .Where(a => a.Timestamp >= last24Hours)
            .CountAsync(cancellationToken);

        return new SecurityOverviewDto
        {
            TotalUserCount = totalUsers,
            ActiveUserCount = activeUsers,
            LockedAccountCount = lockedAccounts,
            UnverifiedAccountCount = unverifiedAccounts,
            FailedLoginAttempts24h = failedLogins,
            SecurityIncidents24h = securityIncidents,
            PendingModerationActions = pendingModerationActions,
            AverageApiRequestsPerMinute = apiRequests / 1440.0, // 24 hours * 60 minutes
            BlockedIpAddresses = blockedIps,
            ActiveSessions = activeSessions
        };
    }
}