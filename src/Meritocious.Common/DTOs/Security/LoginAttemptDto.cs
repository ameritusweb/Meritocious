namespace Meritocious.Common.DTOs.Security;

public class LoginAttemptDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public bool Success { get; set; }
    public string FailureReason { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Location { get; set; }
    public DateTime Timestamp { get; set; }
    public string AuthMethod { get; set; }
    public bool IsSuspicious { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Device { get; set; }
}