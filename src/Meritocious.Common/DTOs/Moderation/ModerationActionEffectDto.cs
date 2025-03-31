using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Moderation
{
    public class ModerationActionEffectDto
    {
        public Guid Id { get; set; }
        public Guid ModerationActionId { get; set; }
        public string EffectType { get; set; }
        public Dictionary<string, string> EffectData { get; set; } = new Dictionary<string, string>();
        public DateTime? ExpiresAt { get; set; }
        public bool IsReverted { get; set; }
        public DateTime? RevertedAt { get; set; }
        public string RevertReason { get; set; }
    }
}
