namespace Meritocious.Core.Entities;

public class QuoteLocation : BaseEntity
{
    public Guid PostSourceId { get; set; }
    public Post PostSource { get; set; }
    public string Text { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; } // Surrounding text for context
}