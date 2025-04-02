namespace Meritocious.Common.DTOs.Content;

public class RemixDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public Guid AuthorId { get; set; }
    public decimal MeritScore { get; set; }
    public List<RemixSourceDto> Sources { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public Guid? SubstackId { get; set; }
    public bool IsDraft { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public SynthesisMapDto SynthesisMap { get; set; }
    public List<RemixNoteDto> Notes { get; set; } = new();
}

public class RemixSourceDto
{
    public Guid SourcePostId { get; set; }
    public string PostTitle { get; set; }
    public string AuthorUsername { get; set; }
    public string Relationship { get; set; }
    public string Context { get; set; }
    public int Order { get; set; }
    public List<QuoteDto> Quotes { get; set; } = new();
    public Dictionary<string, decimal> RelevanceScores { get; set; } = new();
}

public class QuoteDto
{
    public string Text { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; }
}

public class RemixNoteDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public List<Guid> RelatedSourceIds { get; set; } = new();
    public decimal Confidence { get; set; }
    public bool IsApplied { get; set; }
}

public class SynthesisMapDto
{
    public List<SynthesisNodeDto> Nodes { get; set; } = new();
    public List<SynthesisConnectionDto> Connections { get; set; } = new();
}

public class SynthesisNodeDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } // Source, Insight, Connection
    public string Label { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class SynthesisConnectionDto
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public string Type { get; set; } // Support, Contrast, Example, etc.
    public decimal Strength { get; set; }
    public string Label { get; set; }
}