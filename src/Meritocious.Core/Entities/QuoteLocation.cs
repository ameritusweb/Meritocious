using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public class QuoteLocation : BaseEntity<QuoteLocation>
{
    [ForeignKey("FK_PostSourceId")]
    public UlidId<Post> PostSourceId { get; set; }
    public Post PostSource { get; set; }
    public string Content { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Context { get; set; } // Surrounding text for context

    [ForeignKey("FK_PostRelationParentId")]
    public UlidId<Post> PostRelationParentId { get; set; }
    [ForeignKey("FK_PostRelationChildId")]
    public UlidId<Post> PostRelationChildId { get; set; }
    public PostRelation PostRelation { get; set; }
}