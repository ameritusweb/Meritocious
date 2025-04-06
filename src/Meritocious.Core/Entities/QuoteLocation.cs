using Meritocious.Core.Extensions;

namespace Meritocious.Core.Entities;

public class QuoteLocation : BaseEntity<QuoteLocation>
{
    public string PostSourceId { get; set; }
    public Post PostSource { get; set; }
    public string Content { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; } // Surrounding text for context

    public UlidId<Post> PostRelationParentId { get; set; }
    public UlidId<Post> PostRelationChildId { get; set; }
    public PostRelation PostRelation { get; set; }
}