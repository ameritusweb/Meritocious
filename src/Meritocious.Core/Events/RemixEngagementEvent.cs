using System;

namespace Meritocious.Core.Events;

public class RemixEngagementEvent
{
    public RemixEngagementType Type { get; set; }
    
    // View-related properties
    public string Region { get; set; }
    public string Platform { get; set; }
    
    // Interaction-related properties
    public string InteractionType { get; set; }
    
    // Session-related properties
    public decimal TimeSpentSeconds { get; set; }
    public bool Bounced { get; set; }
    
    // Source-related properties
    public Guid? SourceId { get; set; }
    public decimal? InfluenceScore { get; set; }
    
    // Sentiment-related properties
    public decimal? SentimentScore { get; set; }
}

public enum RemixEngagementType
{
    View,
    Interaction,
    SessionEnd,
    Citation,
    Reference
}