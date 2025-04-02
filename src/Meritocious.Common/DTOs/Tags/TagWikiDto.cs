namespace Meritocious.Common.DTOs.Tags;

public class TagWikiDto
{
    public string TagId { get; set; }
    public string Content { get; set; }
    public string LastEditedBy { get; set; }
    public DateTime LastEditedAt { get; set; }
    public string EditSummary { get; set; }
    public int RevisionNumber { get; set; }
}