namespace Meritocious.Core.Entities;

public class QuoteLocation : BaseEntity
{
    public Guid RemixSourceId { get; set; }
    public RemixSource RemixSource { get; set; }
    public string Text { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; } // Surrounding text for context
}