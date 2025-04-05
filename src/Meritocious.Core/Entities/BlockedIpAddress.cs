using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class BlockedIpAddress : BaseEntity
    {
        public string IpAddress { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BlockedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public Guid? BlockedByUserId { get; set; }
        public User? BlockedByUser { get; set; }
    }
}
