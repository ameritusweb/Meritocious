using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public class PostRelation : BaseEntity<PostRelation>
{
    [ForeignKey("FK_ParentId")]
    public UlidId<Post> ParentId { get; private set; }
    [ForeignKey("FK_ChildId")]
    public UlidId<Post> ChildId { get; private set; }
    public string RelationType { get; private set; } // "fork" or "remix"
    public int OrderIndex { get; internal set; } // For ordering remix sources
    public string Role { get; private set; } // For remix: "support", "contrast", "example", "question"
    public string Context { get; private set; } // How this source is used
    public List<QuoteLocation> Quotes { get; set; } = new();
    public decimal RelevanceScore { get; private set; } // AI-generated relevance metric

    public Post Parent { get; private set; }
    public Post Child { get; private set; }

    private PostRelation()
    { 
    } // For EF Core

    public static PostRelation CreateFork(Post parent, Post child)
    {
        return new PostRelation
        {
            ParentId = parent.Id,
            ChildId = child.Id,
            RelationType = "fork",
            Parent = parent,
            Child = child,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static PostRelation CreateRemixSource(
        Post parent, 
        Post remixPost, 
        string role,
        int orderIndex,
        string context = null)
    {
        return new PostRelation
        {
            ParentId = parent.Id,
            ChildId = remixPost.Id,
            RelationType = "remix",
            Role = role,
            OrderIndex = orderIndex,
            Context = context,
            Parent = parent,
            Child = remixPost,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddQuote(QuoteLocation quote)
    {
        Quotes.Add(quote);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRelevanceScore(decimal score)
    {
        RelevanceScore = score;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrderIndex(int newOrder)
    {
        OrderIndex = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(string newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }
}