namespace Meritocious.Core.Entities;

public class Remix : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid AuthorId { get; set; }
    public User Author { get; set; }
    public decimal MeritScore { get; set; }
    public List<RemixSource> Sources { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
    public List<ContentVersion> Versions { get; set; } = new();
    public Guid? SubstackId { get; set; }
    public bool IsDraft { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string SynthesisMap { get; set; } // JSON representation of how sources connect
    public List<RemixNote> Notes { get; set; } = new(); // AI-generated insights/notes
}