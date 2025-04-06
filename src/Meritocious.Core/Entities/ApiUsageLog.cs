using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Meritocious.Core.Entities
{
    public class ApiUsageLog : BaseEntity<ApiUsageLog>
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        [ForeignKey("FK_UserId")]
        public UlidId<User> UserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int DurationMs { get; set; }
        public long ResponseSize { get; set; }
        public Dictionary<string, JsonElement> RequestMetadata { get; set; } = new();
        
        // Navigation properties
        public User? User { get; set; }
    }
}