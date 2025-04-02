namespace Meritocious.Core.Entities
{
    public class LoginAttempt : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public string? FailureReason { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Location { get; set; }
        public string? Device { get; set; }
    }
}