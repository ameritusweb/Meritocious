using System;

namespace Meritocious.Core.Entities
{
    public class ContentSimilarity : BaseEntity<ContentSimilarity>
    {
        public Guid ContentId1 { get; set; }
        public Guid ContentId2 { get; set; }
        public decimal SimilarityScore { get; set; }
        public string Algorithm { get; set; } = "semantic-kernel-v1";
        public DateTime LastUpdated { get; set; }
        public bool NeedsUpdate { get; set; }
        public int UpdatePriority { get; set; } // Higher number = higher priority

        // Navigation properties
        public Post Content1 { get; set; }
        public Post Content2 { get; set; }

        public static ContentSimilarity Create(
            Guid contentId1,
            Guid contentId2,
            decimal similarityScore)
        {
            return new ContentSimilarity
            {
                ContentId1 = contentId1,
                ContentId2 = contentId2,
                SimilarityScore = similarityScore,
                LastUpdated = DateTime.UtcNow,
                NeedsUpdate = false,
                UpdatePriority = 0
            };
        }

        public void MarkForUpdate(int priority = 0)
        {
            NeedsUpdate = true;
            UpdatePriority = priority;
        }

        public void UpdateScore(decimal newScore)
        {
            SimilarityScore = newScore;
            LastUpdated = DateTime.UtcNow;
            NeedsUpdate = false;
            UpdatePriority = 0;
        }
    }
}