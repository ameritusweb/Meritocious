using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class BlockedIpAddress : BaseEntity<BlockedIpAddress>
    {
        public string IpAddress { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BlockedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        [ForeignKey("FK_BlockedByUserId")]
        public UlidId<User> BlockedByUserId { get; set; }
        public User? BlockedByUser { get; set; }
    }
}
