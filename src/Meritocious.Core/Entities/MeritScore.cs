using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class MeritScore : BaseEntity
    {
        public Guid ContentId { get; private set; }
        public string ContentType { get; private set; }
        public Guid ScoreTypeId { get; private set; }
        public decimal Score { get; private set; }

        // Navigation properties
        public MeritScoreType ScoreType { get; private set; }

        private MeritScore()
        {
        } // For EF Core

        public static MeritScore Create(
            Guid contentId,
            string contentType,
            MeritScoreType scoreType,
            decimal score)
        {
            return new MeritScore
            {
                ContentId = contentId,
                ContentType = contentType,
                ScoreTypeId = scoreType.Id,
                ScoreType = scoreType,
                Score = score,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateScore(decimal newScore)
        {
            Score = newScore;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // Extension method for Post/Comment entities
    public static class MeritScoreExtensions
    {
        public static decimal CalculateTotalMeritScore(this IEnumerable<MeritScore> scores)
        {
            return scores
                .Where(s => s.ScoreType.IsActive)
                .Sum(s => s.Score * s.ScoreType.Weight);
        }
    }
}
