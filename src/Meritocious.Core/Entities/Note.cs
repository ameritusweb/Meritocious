using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public class Note : BaseEntity<Note>
{
    [ForeignKey("FK_PostId")]
    public UlidId<Post> PostId { get; private set; }

    public string Type { get; private set; }  // "Connection", "Insight", "Suggestion"

    public string Content { get; private set; }

    public List<string> RelatedSourceIds { get; private set; } = new();

    public decimal Confidence { get; private set; }

    public bool IsApplied { get; private set; }

    public Post Post { get; private set; }

    private Note()
    {
    } // For EF Core

    public static Note Create(
        Post post,
        string type,
        string content,
        List<string> relatedSourceIds,
        decimal confidence)
    {
        return new Note
        {
            PostId = post.Id,
            Post = post,
            Type = type,
            Content = content,
            RelatedSourceIds = relatedSourceIds,
            Confidence = confidence,
            IsApplied = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkApplied(bool isApplied)
    {
        IsApplied = isApplied;
        UpdatedAt = DateTime.UtcNow;
    }
}