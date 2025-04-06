using Meritocious.Common.Enums;
using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class MeritScoreHistory : BaseEntity<MeritScoreHistory>
    {
        public string ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public decimal Score { get; private set; }
        public Dictionary<string, decimal> Components { get; private set; }
        public string ModelVersion { get; private set; }
        public Dictionary<string, string> Explanations { get; private set; }
        public string Context { get; private set; }
        public DateTime EvaluatedAt { get; private set; }
        public bool IsRecalculation { get; private set; }
        public string RecalculationReason { get; private set; }
        public DateTime Timestamp { get; private set; }
        [ForeignKey("FK_UserId")]
        public UlidId<User> UserId { get; private set; }
        public User User { get; private set; }
        public string Reason { get; private set; }

        private MeritScoreHistory()
        {
            Components = new Dictionary<string, decimal>();
            Explanations = new Dictionary<string, string>();
        }

        public static MeritScoreHistory Create(
            string contentId,
            ContentType contentType,
            decimal score,
            Dictionary<string, decimal> components,
            string modelVersion,
            Dictionary<string, string> explanations,
            string context = null,
            bool isRecalculation = false,
            string recalculationReason = null)
        {
            return new MeritScoreHistory
            {
                ContentId = contentId,
                ContentType = contentType,
                Score = score,
                Components = components,
                ModelVersion = modelVersion,
                Explanations = explanations,
                Context = context,
                EvaluatedAt = DateTime.UtcNow,
                IsRecalculation = isRecalculation,
                RecalculationReason = recalculationReason,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
