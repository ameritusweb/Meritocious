using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ModerationActionEffect : BaseEntity
    {
        public Guid ModerationActionId { get; private set; }
        public ModerationAction ModerationAction { get; private set; }
        public string EffectType { get; private set; }
        public Dictionary<string, string> EffectData { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsReverted { get; private set; }
        public DateTime? RevertedAt { get; private set; }
        public string RevertReason { get; private set; }

        private ModerationActionEffect()
        {
            EffectData = new Dictionary<string, string>();
        }

        public static ModerationActionEffect Create(
            ModerationAction action,
            string effectType,
            Dictionary<string, string> effectData,
            DateTime? expiresAt = null)
        {
            return new ModerationActionEffect
            {
                ModerationActionId = action.Id,
                ModerationAction = action,
                EffectType = effectType,
                EffectData = effectData,
                ExpiresAt = expiresAt,
                IsReverted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Revert(string reason)
        {
            IsReverted = true;
            RevertedAt = DateTime.UtcNow;
            RevertReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
