namespace Meritocious.Common.DTOs.Tags;

public class TagDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UsageCount { get; set; }
    public int FollowerCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public bool IsModerated { get; set; }
}