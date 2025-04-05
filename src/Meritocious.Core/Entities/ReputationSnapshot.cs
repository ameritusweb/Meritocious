using Meritocious.Core.Features.Reputation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ReputationSnapshot : BaseEntity<ReputationSnapshot>
    {
        public string UserId { get; private set; }
        public User User { get; private set; }
        public decimal OverallMeritScore { get; private set; }
        public Dictionary<string, decimal> MetricSnapshots { get; private set; }
        public ReputationLevel Level { get; private set; }
        public string TimeFrame { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private ReputationSnapshot()
        {
            MetricSnapshots = new Dictionary<string, decimal>();
        }

        public static ReputationSnapshot Create(
            User user,
            decimal overallScore,
            Dictionary<string, decimal> metrics,
            ReputationLevel level,
            string timeFrame,
            DateTime startDate,
            DateTime endDate)
        {
            return new ReputationSnapshot
            {
                UserId = user.Id,
                User = user,
                OverallMeritScore = overallScore,
                MetricSnapshots = metrics,
                Level = level,
                TimeFrame = timeFrame,
                StartDate = startDate,
                EndDate = endDate,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
