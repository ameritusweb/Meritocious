using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Common.DTOs.Security;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetSecurityOverviewQueryHandler : IRequestHandler<GetSecurityOverviewQuery, SecurityOverviewDto>
{
    private readonly MeritociousDbContext _context;

    public GetSecurityOverviewQueryHandler(MeritociousDbContext context)
    {
        _context = context;
    }

    public async Task<SecurityOverviewDto> Handle(GetSecurityOverviewQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var last24Hours = now.AddHours(-24);

        var totalUsers = await _context.Users.CountAsync(cancellationToken);
        var activeUsers = await _context.Users.CountAsync(u => u.LastActivityAt >= last24Hours, cancellationToken);
        var lockedAccounts = await _context.Users.CountAsync(u => u.IsLocked, cancellationToken);
        var unverifiedAccounts = await _context.Users.CountAsync(u => !u.IsEmailVerified, cancellationToken);

        var failedLogins = await _context.LoginAttempts
            .CountAsync(l => !l.Success && l.Timestamp >= last24Hours, cancellationToken);

        var securityIncidents = await _context.SecurityEvents
            .CountAsync(e => e.Timestamp >= last24Hours, cancellationToken);

        var pendingModerationActions = await _context.ModerationActions
            .CountAsync(m => m.Status == "Pending", cancellationToken);

        var blockedIps = await _context.BlockedIpAddresses
            .CountAsync(b => b.ExpiresAt > now, cancellationToken);

        var activeSessions = await _context.UserSessions
            .CountAsync(s => s.LastActivityAt >= last24Hours, cancellationToken);

        var apiRequests = await _context.ApiUsageLogs
            .Where(a => a.TimeStamp >= last24Hours)
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