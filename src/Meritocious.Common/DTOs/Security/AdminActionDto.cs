namespace Meritocious.Common.DTOs.Security;

public class AdminActionDto
{
    public string Id { get; set; }
    public string AdminId { get; set; }
    public string AdminName { get; set; }
    public string ActionType { get; set; }
    public string TargetType { get; set; }
    public string TargetId { get; set; }
    public string Details { get; set; }
    public string Reason { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string Status { get; set; }
}