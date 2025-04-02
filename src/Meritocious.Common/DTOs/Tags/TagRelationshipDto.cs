namespace Meritocious.Common.DTOs.Tags;

public class TagRelationshipDto
{
    public string ParentTagId { get; set; }
    public string ChildTagId { get; set; }
    public string RelationType { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}