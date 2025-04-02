namespace Meritocious.Common.DTOs.Tags;

public class TagModerationLogDto
{
    public string Id { get; set; }
    public string TagId { get; set; }
    public string ModeratorId { get; set; }
    public string Action { get; set; }
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PreviousState { get; set; }
    public string NewState { get; set; }
}