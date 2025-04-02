namespace Meritocious.Core.Entities
{
    public class AdminActionLog : BaseEntity
    {
        public Guid AdminUserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        
        // Navigation properties
        public User AdminUser { get; set; } = null!;
    }
}