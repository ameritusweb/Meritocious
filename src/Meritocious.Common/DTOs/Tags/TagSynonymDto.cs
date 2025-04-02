namespace Meritocious.Common.DTOs.Tags;

public class TagSynonymDto
{
    public string SourceTagId { get; set; }
    public string TargetTagId { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string Status { get; set; }
}