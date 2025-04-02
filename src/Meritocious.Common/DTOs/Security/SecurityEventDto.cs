namespace Meritocious.Common.DTOs.Security;

public class SecurityEventDto
{
    public string Id { get; set; }
    public string EventType { get; set; }
    public string Severity { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public string RelatedEntityType { get; set; }
    public string RelatedEntityId { get; set; }
    public bool RequiresAction { get; set; }
    public string Status { get; set; }
}