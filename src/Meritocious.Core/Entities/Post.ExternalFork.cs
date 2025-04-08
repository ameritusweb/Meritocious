using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public partial class Post
{
    [ForeignKey("FK_ForkTypeId")]
    public UlidId<ForkType>? ForkTypeId { get; private set; }
    public ForkType? ForkType { get; private set; }

    [ForeignKey("FK_ExternalForkSourceId")]
    public UlidId<ExternalForkSource>? ExternalForkSourceId { get; private set; }
    public ExternalForkSource? ExternalForkSource { get; private set; }

    public string? ExternalForkContext { get; private set; }
    public string? ExternalForkQuote { get; private set; }

    public static Post CreateExternalFork(
        string title,
        string content,
        User author,
        ForkType forkType,
        ExternalForkSource source,
        string? context = null,
        string? quote = null,
        Substack? substack = null)
    {
        var post = Create(title, content, author, null, substack);
        post.ForkTypeId = forkType.Id;
        post.ForkType = forkType;
        post.ExternalForkSourceId = source.Id;
        post.ExternalForkSource = source;
        post.ExternalForkContext = context;
        post.ExternalForkQuote = quote;
        return post;
    }

    public void UpdateExternalForkMetadata(
        string? context = null,
        string? quote = null)
    {
        if (ExternalForkSourceId == null)
        {
            throw new InvalidOperationException("This post is not an external fork");
        }

        ExternalForkContext = context;
        ExternalForkQuote = quote;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExternalFork => ExternalForkSourceId != null;
}