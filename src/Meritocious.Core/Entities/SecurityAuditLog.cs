using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Meritocious.Core.Entities
{
    public class SecurityAuditLog : BaseEntity<SecurityAuditLog>
    {
        public string EventType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        [ForeignKey("FK_UserId")]
        public UlidId<User>? UserId { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, JsonElement> Context { get; set; } = new();

        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? AdditionalData { get; set; }

        public User? User { get; set; }
    }
}