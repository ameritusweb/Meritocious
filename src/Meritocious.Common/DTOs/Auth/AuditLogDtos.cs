namespace Meritocious.Common.DTOs.Auth
{
    public class AdminActionLogDto
    {
        public Guid Id { get; set; }
        public string AdminUsername { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class SecurityAuditLogDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public class LoginAttemptDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public string? FailureReason { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Location { get; set; }
        public string? Device { get; set; }
    }

    public class ApiUsageLogDto
    {
        public Guid Id { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Username { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int DurationMs { get; set; }
        public long ResponseSize { get; set; }
        public Dictionary<string, object> RequestMetadata { get; set; } = new();
    }

    public class AuditLogQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Category { get; set; }
        public string? Username { get; set; }
        public string? IpAddress { get; set; }
    }

    public class SecurityOverviewDto
    {
        public int FailedLoginAttempts24h { get; set; }
        public int UniqueSuspiciousIps24h { get; set; }
        public int SecurityIncidents24h { get; set; }
        public int TotalApiRequests24h { get; set; }
        public List<LoginAttemptDto> RecentFailedLogins { get; set; } = new();
        public List<SecurityAuditLogDto> RecentSecurityEvents { get; set; } = new();
        public Dictionary<string, int> ApiEndpointUsage { get; set; } = new();
    }
}