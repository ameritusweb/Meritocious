using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ReputationBadge : BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public string BadgeType { get; private set; }
        public string Category { get; private set; }
        public int Level { get; private set; }
        public Dictionary<string, string> Criteria { get; private set; }
        public Dictionary<string, decimal> Progress { get; private set; }
        public DateTime? AwardedAt { get; private set; }
        public string AwardReason { get; private set; }

        private ReputationBadge()
        {
            Criteria = new Dictionary<string, string>();
            Progress = new Dictionary<string, decimal>();
        }

        public static ReputationBadge Create(
            User user,
            string badgeType,
            string category,
            int level,
            Dictionary<string, string> criteria)
        {
            return new ReputationBadge
            {
                UserId = Guid.Parse(user.Id),
                User = user,
                BadgeType = badgeType,
                Category = category,
                Level = level,
                Criteria = criteria,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateProgress(Dictionary<string, decimal> progress)
        {
            Progress = progress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Award(string reason)
        {
            AwardedAt = DateTime.UtcNow;
            AwardReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
