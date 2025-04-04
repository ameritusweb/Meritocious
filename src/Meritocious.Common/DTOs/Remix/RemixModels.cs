using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Remix
{
    public class CreateRemixRequest
    {
        public string Title { get; set; }
        public string InitialContent { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? SubstackId { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<Guid> InitialSourceIds { get; set; } = new();
    }

    public class UpdateRemixRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public bool UpdateSynthesisMap { get; set; }
    }

    public class AddSourceRequest
    {
        public Guid PostId { get; set; }
        public string Relationship { get; set; }
        public string Context { get; set; }
        public List<string> InitialQuotes { get; set; }
    }

    public class SourceOrderUpdate
    {
        public Guid SourceId { get; set; }
        public int NewOrder { get; set; }
    }

    public class AddQuoteRequest
    {
        public string Text { get; set; }
        public string Context { get; set; }
    }

    public class RemixFilter
    {
        public bool IncludeDrafts { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public string SortBy { get; set; } = "newest";
    }

    public class RemixSearchRequest
    {
        public string Query { get; set; }
        public List<string> Tags { get; set; } = new();
        public decimal? MinMeritScore { get; set; }
        public string SortBy { get; set; } = "relevance";
        public int? Limit { get; set; }
    }

    public class RemixScoreResult
    {
        public decimal FinalScore { get; set; }
        public decimal SynthesisScore { get; set; }
        public decimal CohesionScore { get; set; }
        public decimal NoveltyScore { get; set; }
        public decimal SourceUtilizationScore { get; set; }
        public List<ScoreInsight> Insights { get; set; } = new();
    }

    public class ScoreInsight
    {
        public string Category { get; set; }
        public decimal Score { get; set; }
        public string Explanation { get; set; }
    }

    public enum RemixInteractionType
    {
        View,
        Like,
        Comment,
        Share,
        Fork,
        Quote
    }

    public class RemixAnalytics
    {
        public int TotalSources { get; set; }
        public Dictionary<string, int> RelationshipDistribution { get; set; }
        public Dictionary<string, int> NoteTypeDistribution { get; set; }
        public decimal AverageSourceRelevance { get; set; }
        public List<string> TopTags { get; set; }
        public RemixEngagementMetrics Engagement { get; set; }
    }

    public class RemixEngagementMetrics
    {
        public int Views { get; set; }
        public int UniqueViews { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Forks { get; set; }
        public int Shares { get; set; }
        public decimal AverageTimeSpentMinutes { get; set; }
        public decimal BounceRate { get; set; }
        public int ContributorCount { get; set; }
        public int CitationCount { get; set; }
        public int ReferenceCount { get; set; }
        public Dictionary<string, int> ViewsByRegion { get; set; } = new();
        public Dictionary<string, int> ViewsByPlatform { get; set; } = new();
        public Dictionary<DateTime, int> ViewTrend { get; set; } = new();
        public Dictionary<string, decimal> SourceInfluenceScores { get; set; } = new();
        public DateTime PeakEngagementTime { get; set; }
        public decimal EngagementVelocity { get; set; }
        public decimal SentimentScore { get; set; }
        public List<string> TopEngagementSources { get; set; } = new();
    }
}
