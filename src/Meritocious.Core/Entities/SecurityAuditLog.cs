namespace Meritocious.Core.Entities
{
    public class SecurityAuditLog : BaseEntity
    {
        public string EventType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
        
        // Navigation properties
        public User? User { get; set; }
    }
}