namespace Meritocious.Core.Entities;

public class QuoteLocation : BaseEntity<QuoteLocation>
{
    public string PostSourceId { get; set; }
    public Post PostSource { get; set; }
    public string Content { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; } // Surrounding text for context

    public string PostRelationParentId { get; set; }
    public string PostRelationChildId { get; set; }
    public PostRelation PostRelation { get; set; }
}