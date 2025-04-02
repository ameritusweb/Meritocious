using System;

namespace Meritocious.Core.Entities;

public class RemixEngagement : BaseEntity
{
    public Guid RemixId { get; set; }
    public Remix Remix { get; set; }
    
    // View tracking
    public int TotalViews { get; set; }
    public int UniqueViews { get; set; }
    public Dictionary<string, int> ViewsByRegion { get; set; } = new();
    public Dictionary<string, int> ViewsByPlatform { get; set; } = new();
    
    // Interaction metrics
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Forks { get; set; }
    public int Shares { get; set; }
    public decimal AverageTimeSpentSeconds { get; set; }
    public decimal BounceRate { get; set; } // % of users who leave quickly
    
    // Content impact
    public int CitationCount { get; set; } // Times cited by other remixes
    public int ReferenceCount { get; set; } // Times referenced without direct citation
    public Dictionary<Guid, decimal> SourceInfluenceScores { get; set; } = new();
    
    // Community interaction
    public int ThreadDepth { get; set; } // Max depth of comment threads
    public int ContributorCount { get; set; } // Unique users who engaged
    public decimal SentimentScore { get; set; } // Average sentiment of comments
    
    // Time-based metrics
    public Dictionary<DateTime, int> ViewsOverTime { get; set; } = new();
    public DateTime PeakEngagementTime { get; set; }
    public decimal EngagementVelocity { get; set; } // Rate of engagement growth

    public void RecordView(string region, string platform)
    {
        TotalViews++;
        ViewsByRegion[region] = ViewsByRegion.GetValueOrDefault(region) + 1;
        ViewsByPlatform[platform] = ViewsByPlatform.GetValueOrDefault(platform) + 1;
        
        var today = DateTime.UtcNow.Date;
        ViewsOverTime[today] = ViewsOverTime.GetValueOrDefault(today) + 1;
    }

    public void UpdateSourceInfluence(Guid sourceId, decimal score)
    {
        SourceInfluenceScores[sourceId] = score;
    }

    public void RecordInteraction(RemixInteractionType type)
    {
        switch (type)
        {
            case RemixInteractionType.Like:
                Likes++;
                break;
            case RemixInteractionType.Comment:
                Comments++;
                break;
            case RemixInteractionType.Fork:
                Forks++;
                break;
            case RemixInteractionType.Share:
                Shares++;
                break;
        }
        ContributorCount++; // Assuming unique contributors
    }

    public void UpdateEngagementMetrics(decimal timeSpent, bool bounced)
    {
        // Update moving averages
        var oldTotal = AverageTimeSpentSeconds * (TotalViews - 1);
        AverageTimeSpentSeconds = (oldTotal + timeSpent) / TotalViews;

        if (bounced)
        {
            var oldBounces = BounceRate * (TotalViews - 1);
            BounceRate = (oldBounces + 1) / TotalViews;
        }

        // Update peak engagement if current engagement is higher
        var currentEngagement = CalculateCurrentEngagement();
        if (currentEngagement > EngagementVelocity)
        {
            EngagementVelocity = currentEngagement;
            PeakEngagementTime = DateTime.UtcNow;
        }
    }

    private decimal CalculateCurrentEngagement()
    {
        const int RECENT_DAYS = 7;
        var recentDate = DateTime.UtcNow.AddDays(-RECENT_DAYS);
        
        var recentViews = ViewsOverTime
            .Where(v => v.Key >= recentDate)
            .Sum(v => v.Value);

        return (decimal)(
            recentViews * 1.0 +
            Likes * 2.0 +
            Comments * 3.0 +
            Forks * 4.0 +
            Shares * 2.0
        ) / RECENT_DAYS;
    }
}

public enum RemixInteractionType
{
    Like,
    Comment,
    Fork,
    Share
}