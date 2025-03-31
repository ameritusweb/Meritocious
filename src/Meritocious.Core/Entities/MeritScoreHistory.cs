using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class MeritScoreHistory : BaseEntity
    {
        public Guid ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public decimal Score { get; private set; }
        public string ModelVersion { get; private set; }
        public Dictionary<string, decimal> ComponentScores { get; private set; }
        public DateTime Timestamp { get; private set; }

        private MeritScoreHistory()
        {
            ComponentScores = new Dictionary<string, decimal>();
        }

        public static MeritScoreHistory Create(
            Guid contentId,
            ContentType contentType,
            decimal score,
            string modelVersion,
            Dictionary<string, decimal> componentScores)
        {
            return new MeritScoreHistory
            {
                ContentId = contentId,
                ContentType = contentType,
                Score = score,
                ModelVersion = modelVersion,
                ComponentScores = componentScores,
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
