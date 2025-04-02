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
}