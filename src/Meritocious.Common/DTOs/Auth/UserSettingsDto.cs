namespace Meritocious.Common.DTOs.Auth;

public class UserSettingsDto
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool PublicProfile { get; set; }
    public string[]? PreferredTags { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
}