namespace Meritocious.Core.Entities;

public class RemixSource : BaseEntity
{
    public Guid RemixId { get; set; }
    public Remix Remix { get; set; }
    public Guid SourcePostId { get; set; }
    public Post SourcePost { get; set; }
    public string Relationship { get; set; } // Support, Contrast, Example, Question
    public string Context { get; set; } // Explanation of how this source is used
    public int Order { get; set; } // Order in the synthesis flow
    public List<string> QuotedExcerpts { get; set; } = new(); // Specific quotes used from this source
    public Dictionary<string, decimal> RelevanceScores { get; set; } = new(); // AI-generated relevance metrics
}