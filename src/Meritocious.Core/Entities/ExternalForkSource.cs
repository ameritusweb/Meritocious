using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Meritocious.Core.Entities;

public class ExternalForkSource : BaseEntity<ExternalForkSource>
{
    public string Type { get; private set; } // reddit, hn, substack, youtube, etc.
    public string Platform { get; private set; } // Platform display name
    public string Title { get; private set; }
    public string SourceUrl { get; private set; }
    public string AuthorName { get; private set; }
    public DateTime? Timestamp { get; private set; }

    // Context fields
    public string Subtype { get; private set; } // comment, post, chapter, scene, clip, etc.
    public Dictionary<string, JsonElement> LocationMetadata { get; private set; } = new(); // Flexible storage for location data

    // Tags for categorization
    public List<string> Tags { get; private set; } = new();

    // Additional metadata
    public Dictionary<string, JsonElement> AdditionalMetadata { get; private set; } = new();

    // Navigation properties
    private readonly List<Post> forks = new();
    public IReadOnlyCollection<Post> Forks => forks.AsReadOnly();

    private readonly List<ForkRequest> requests = new();
    public IReadOnlyCollection<ForkRequest> Requests => requests.AsReadOnly();

    private ExternalForkSource()
    {
    }

    public static ExternalForkSource Create(
        string type,
        string platform,
        string title,
        string sourceUrl,
        string authorName,
        Dictionary<string, JsonElement> locationMetadata,
        Dictionary<string, JsonElement> additionalMetadata,
        DateTime? timestamp = null,
        string subtype = null,
        List<string>? tags = null)
    {
        return new ExternalForkSource
        {
            Type = type,
            Platform = platform,
            Title = title,
            SourceUrl = sourceUrl,
            AuthorName = authorName,
            Timestamp = timestamp,
            Subtype = subtype,
            LocationMetadata = locationMetadata,
            Tags = tags ?? new List<string>(),
            AdditionalMetadata = additionalMetadata,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateMetadata(Dictionary<string, JsonElement> metadata)
    {
        AdditionalMetadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocationMetadata(Dictionary<string, JsonElement> metadata)
    {
        LocationMetadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTag(string tag)
    {
        if (Tags.Remove(tag))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}