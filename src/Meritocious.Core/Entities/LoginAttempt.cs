namespace Meritocious.Core.Entities
{
    public class LoginAttempt : BaseEntity<LoginAttempt>
    {
        public string Username { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool IsSuspicious { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserId { get; set; }
        public string? UserAgent { get; set; }
        public string? FailureReason { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AuthMethod { get; set; }
        public string? Location { get; set; }
        public string? Device { get; set; }
    }
}