namespace Meritocious.Core.Entities;

public class RemixNote : BaseEntity
{
    public Guid RemixId { get; set; }
    public Remix Remix { get; set; }
    public string Type { get; set; } // Insight, Connection, Suggestion, etc.
    public string Content { get; set; }
    public List<Guid> RelatedSourceIds { get; set; } = new(); // Sources this note relates to
    public decimal Confidence { get; set; } // AI confidence in this insight
    public bool IsApplied { get; set; } // Whether the author used this note
}