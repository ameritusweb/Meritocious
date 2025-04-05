using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public partial class Post : BaseEntity<Post>
    {
        private PostEngagement engagement;
        public PostEngagement Engagement => engagement ?? (engagement = PostEngagement.Create(this));

        // Basic engagement methods
        public void RecordView(string region, string platform, bool isUnique, decimal timeSpentSeconds, bool bounced)
        {
            Engagement.RecordView(region, platform, isUnique, timeSpentSeconds, bounced);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLike()
        {
            Engagement.RecordLike();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordComment()
        {
            Engagement.RecordComment();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordFork()
        {
            Engagement.RecordFork();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordShare()
        {
            Engagement.RecordShare();
            UpdatedAt = DateTime.UtcNow;
        }

        // Impact tracking methods
        public void RecordCitation()
        {
            Engagement.RecordCitation();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordReference()
        {
            Engagement.RecordReference();
            UpdatedAt = DateTime.UtcNow;
        }

        // Update methods
        public void UpdateSourceInfluence(string sourceId, decimal score)
        {
            Engagement.UpdateSourceInfluence(sourceId, score);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContributorCount(int count)
        {
            Engagement.UpdateContributorCount(count);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSentimentScore(decimal score)
        {
            Engagement.UpdateSentimentScore(score);
            UpdatedAt = DateTime.UtcNow;
        }

        // Read-only metric properties
        public int ViewCount => Engagement.Views;
        public int UniqueViewCount => Engagement.UniqueViews;
        public int LikeCount => Engagement.Likes;
        public int CommentCount => Engagement.Comments;
        public int ForkCount => Engagement.Forks;
        public int ShareCount => Engagement.Shares;
        public int CitationCount => Engagement.CitationCount;
        public int ReferenceCount => Engagement.ReferenceCount;
        public decimal AverageTimeSpentMinutes => Engagement.AverageTimeSpentSeconds / 60m;
        public decimal BounceRate => Engagement.BounceRate;
        public int ContributorCount => Engagement.ContributorCount;
        public decimal EngagementVelocity => Engagement.EngagementVelocity;
        public decimal SentimentScore => Engagement.SentimentScore;
        public DateTime? PeakEngagementTime => Engagement.PeakEngagementTime;

        // Read-only breakdown properties
        public IReadOnlyDictionary<string, int> ViewsByRegion =>
            new Dictionary<string, int>(Engagement.ViewsByRegion);
        public IReadOnlyDictionary<string, int> ViewsByPlatform =>
            new Dictionary<string, int>(Engagement.ViewsByPlatform);
        public IReadOnlyDictionary<DateTime, int> ViewTrend =>
            new Dictionary<DateTime, int>(Engagement.ViewTrend);
        public IReadOnlyDictionary<string, decimal> SourceInfluenceScores =>
            new Dictionary<string, decimal>(Engagement.SourceInfluenceScores);
        public IReadOnlyList<string> TopEngagementSources =>
            new List<string>(Engagement.TopEngagementSources);
    }
}
