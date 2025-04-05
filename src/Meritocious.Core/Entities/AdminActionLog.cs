using System.Text.Json;

namespace Meritocious.Core.Entities
{
    public class AdminActionLog : BaseEntity
    {
        public string AdminUserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, JsonElement> Metadata { get; set; } = new();
        
        // Navigation properties
        public User AdminUser { get; set; } = null!;
    }
}