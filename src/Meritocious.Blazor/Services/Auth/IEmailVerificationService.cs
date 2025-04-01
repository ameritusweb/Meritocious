namespace Meritocious.Blazor.Services.Auth
{
    public interface IEmailVerificationService
    {
        Task<bool> SendVerificationEmailAsync(string email);
        Task<bool> VerifyEmailAsync(string token);
        Task<bool> ResendVerificationEmailAsync(string email);
    }

}
