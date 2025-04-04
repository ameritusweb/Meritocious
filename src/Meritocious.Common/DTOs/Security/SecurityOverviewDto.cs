using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Common.DTOs.Security;

public class SecurityOverviewDto
{
    public int TotalUserCount { get; set; }
    public int ActiveUserCount { get; set; }
    public int LockedAccountCount { get; set; }
    public int UnverifiedAccountCount { get; set; }
    public int FailedLoginAttempts24h { get; set; }
    public int SecurityIncidents24h { get; set; }
    public int PendingModerationActions { get; set; }
    public double AverageApiRequestsPerMinute { get; set; }
    public int BlockedIpAddresses { get; set; }
    public int ActiveSessions { get; set; }
    public int UniqueSuspiciousIps24h { get; set; }
    public int TotalApiRequests24h { get; set; }
    public List<LoginAttemptDto> RecentFailedLogins { get; set; } = new();
    public List<SecurityAuditLogDto> RecentSecurityEvents { get; set; } = new();
    public Dictionary<string, int> ApiEndpointUsage { get; set; } = new();
}