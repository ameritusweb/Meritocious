using System;
using System.Collections.Generic;

namespace Meritocious.Common.DTOs.Engagement;

public class RemixEngagementMetricsDto
{
    // Basic metrics
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Forks { get; set; }
    public int Shares { get; set; }
    
    // User engagement
    public decimal AverageTimeSpentMinutes { get; set; }
    public decimal BounceRate { get; set; }
    public int ContributorCount { get; set; }
    
    // Impact metrics
    public int CitationCount { get; set; }
    public int ReferenceCount { get; set; }
    
    // Breakdown
    public Dictionary<string, int> ViewsByRegion { get; set; }
    public Dictionary<string, int> ViewsByPlatform { get; set; }
    public Dictionary<DateTime, int> ViewTrend { get; set; }
    public Dictionary<string, decimal> SourceInfluenceScores { get; set; }
    
    // Insights
    public DateTime PeakEngagementTime { get; set; }
    public decimal EngagementVelocity { get; set; }
    public decimal SentimentScore { get; set; }
    public List<string> TopEngagementSources { get; set; }
}