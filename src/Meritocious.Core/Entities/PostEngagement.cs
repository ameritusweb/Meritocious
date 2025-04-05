using Meritocious.Common.DTOs.Engagement;
using Meritocious.Common.DTOs.Remix;

namespace Meritocious.Core.Entities
{
    public class PostEngagement : BaseEntity<PostEngagement>
    {
        // Basic metrics
        public int Views { get; internal set; }
        public int UniqueViews { get; internal set; }
        public int Likes { get; internal set; }
        public int Comments { get; internal set; }
        public int Forks { get; internal set; }
        public int Shares { get; internal set; }

        // User engagement
        public decimal AverageTimeSpentSeconds { get; internal set; }
        public decimal BounceRate { get; internal set; }
        public int ContributorCount { get; internal set; }

        // Impact metrics
        public int CitationCount { get; internal set; }
        public int ReferenceCount { get; internal set; }

        // Breakdowns
        public Dictionary<string, int> ViewsByRegion { get; internal set; } = new();
        public Dictionary<string, int> ViewsByPlatform { get; internal set; } = new();
        public Dictionary<DateTime, int> ViewTrend { get; internal set; } = new();
        public Dictionary<string, decimal> SourceInfluenceScores { get; internal set; } = new();
        public int TotalViews => Views;
        public Dictionary<DateTime, int> ViewsOverTime => ViewTrend;

        // Insights
        public DateTime? PeakEngagementTime { get; internal set; }
        public decimal EngagementVelocity { get; internal set; }
        public decimal SentimentScore { get; internal set; }
        public List<string> TopEngagementSources { get; internal set; } = new();

        // Navigation properties
        public Guid PostId { get; internal set; }
        public Post Post { get; internal set; }

        internal PostEngagement()
        {
        }

        public static PostEngagement Create(Post post)
        {
            return new PostEngagement
            {
                PostId = post.Id,
                Post = post,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void RecordView(string region, string platform, bool isUnique, decimal timeSpentSeconds, bool bounced)
        {
            Views++;

            if (isUnique)
            {
                UniqueViews++;
            }

            // Update region stats
            if (!ViewsByRegion.ContainsKey(region))
            {
                ViewsByRegion[region] = 0;
            }

            ViewsByRegion[region]++;

            // Update platform stats
            if (!ViewsByPlatform.ContainsKey(platform))
            {
                ViewsByPlatform[platform] = 0;
            }

            ViewsByPlatform[platform]++;

            // Update time trend
            var timeKey = DateTime.UtcNow.Date;
            if (!ViewTrend.ContainsKey(timeKey))
            {
                ViewTrend[timeKey] = 0;
            }

            ViewTrend[timeKey]++;

            // Update average time spent
            var oldTotal = AverageTimeSpentSeconds * (Views - 1);
            AverageTimeSpentSeconds = (oldTotal + timeSpentSeconds) / Views;

            // Update bounce rate
            if (bounced)
            {
                var totalBounces = BounceRate * (Views - 1) + 1;
                BounceRate = totalBounces / Views;
            }

            // Update peak engagement time if this is the highest daily view count
            if (!ViewTrend.ContainsKey(timeKey) || ViewTrend[timeKey] > ViewTrend.Values.Max())
            {
                PeakEngagementTime = timeKey;
            }

            // Calculate engagement velocity (views per hour over last 24h)
            var last24Hours = ViewTrend
                .Where(kv => kv.Key >= DateTime.UtcNow.AddHours(-24))
                .Sum(kv => kv.Value);
            EngagementVelocity = last24Hours / 24m;

            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLike()
        {
            Likes++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordComment()
        {
            Comments++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordFork()
        {
            Forks++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordShare()
        {
            Shares++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordCitation()
        {
            CitationCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordReference()
        {
            ReferenceCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordInteraction(RemixInteractionType type)
        {
            switch (type)
            {
                case RemixInteractionType.Like:
                    RecordLike();
                    break;
                case RemixInteractionType.Comment:
                    RecordComment();
                    break;
                case RemixInteractionType.Share:
                    RecordShare();
                    break;
                case RemixInteractionType.Fork:
                    RecordFork();
                    break;
            }
        }

        public void UpdateEngagementMetrics(decimal timeSpentSeconds, bool bounced)
        {
            AverageTimeSpentSeconds = ((AverageTimeSpentSeconds * (Views - 1)) + timeSpentSeconds) / Views;
            if (bounced)
            {
                BounceRate = ((BounceRate * (Views - 1)) + 1) / Views;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSourceInfluence(string sourceId, decimal score)
        {
            SourceInfluenceScores[sourceId] = score;

            // Update top engagement sources
            TopEngagementSources = SourceInfluenceScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(5)
                .Select(kvp => kvp.Key)
                .ToList();

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContributorCount(int count)
        {
            ContributorCount = count;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSentimentScore(decimal score)
        {
            SentimentScore = score;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // Extension method to convert to DTO
    public static class PostEngagementExtensions
    {
        public static RemixEngagementMetricsDto ToDto(this PostEngagement engagement)
        {
            return new RemixEngagementMetricsDto
            {
                Views = engagement.Views,
                UniqueViews = engagement.UniqueViews,
                Likes = engagement.Likes,
                Comments = engagement.Comments,
                Forks = engagement.Forks,
                Shares = engagement.Shares,

                AverageTimeSpentMinutes = engagement.AverageTimeSpentSeconds / 60m,
                BounceRate = engagement.BounceRate,
                ContributorCount = engagement.ContributorCount,

                CitationCount = engagement.CitationCount,
                ReferenceCount = engagement.ReferenceCount,

                ViewsByRegion = new Dictionary<string, int>(engagement.ViewsByRegion),
                ViewsByPlatform = new Dictionary<string, int>(engagement.ViewsByPlatform),
                ViewTrend = new Dictionary<DateTime, int>(engagement.ViewTrend),
                SourceInfluenceScores = new Dictionary<string, decimal>(engagement.SourceInfluenceScores),

                PeakEngagementTime = engagement.PeakEngagementTime ?? DateTime.MinValue,
                EngagementVelocity = engagement.EngagementVelocity,
                SentimentScore = engagement.SentimentScore,
                TopEngagementSources = new List<string>(engagement.TopEngagementSources)
            };
        }
    }
}