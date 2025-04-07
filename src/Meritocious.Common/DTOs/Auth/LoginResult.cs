namespace Meritocious.Common.DTOs.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Error { get; set; }
        public bool RequiresGoogleLink { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public UserSettingsDto User { get; set; }
    }
}