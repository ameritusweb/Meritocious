using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;
using Meritocious.Common.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Meritocious.Infrastructure.Services
{
    public class AuthenticationService : Core.Interfaces.IAuthenticationService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly GoogleAuthSettings googleSettings;
        private readonly ILogger<AuthenticationService> logger;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthenticationService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<GoogleAuthSettings> googleSettings,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthenticationService> logger)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.googleSettings = googleSettings.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public async Task<Result<AuthenticationResult>> AuthenticateGoogleUserAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleSettings.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check if user exists with this Google ID
                var externalLogin = await userRepository.GetExternalLoginAsync("Google", payload.Subject);

                if (externalLogin != null)
                {
                    // Update existing login info
                    externalLogin.UpdateLoginInfo(
                        payload.Name,
                        payload.Picture,
                        tokenService.GenerateRefreshToken(),
                        DateTime.UtcNow.AddDays(30));

                    await userRepository.UpdateExternalLoginAsync(externalLogin);

                    return Result.Success(new AuthenticationResult
                    {
                        AccessToken = tokenService.GenerateAccessToken(externalLogin.User),
                        RefreshToken = externalLogin.RefreshToken,
                        ExpiresAt = tokenService.GetAccessTokenExpiration(),
                        User = externalLogin.User.ToDto(),
                        IsNewUser = false
                    });
                }

                // Check if user exists with this email
                var user = await userRepository.GetByEmailAsync(payload.Email);
                bool isNewUser = false;

                if (user == null)
                {
                    // Create new user
                    user = User.Create(
                        payload.Email.Split('@')[0], // Use email prefix as username
                        payload.Email,
                        Guid.NewGuid().ToString()); // Random password for external accounts

                    await userRepository.AddAsync(user);
                    isNewUser = true;
                }

                // Create external login
                var newExternalLogin = ExternalLogin.Create(
                    user,
                    "Google",
                    payload.Subject,
                    payload.Email,
                    payload.Name,
                    payload.Picture,
                    tokenService.GenerateRefreshToken(),
                    DateTime.UtcNow.AddDays(30));

                await userRepository.AddExternalLoginAsync(newExternalLogin);

                return Result.Success(new AuthenticationResult
                {
                    AccessToken = tokenService.GenerateAccessToken(user),
                    RefreshToken = newExternalLogin.RefreshToken,
                    ExpiresAt = tokenService.GetAccessTokenExpiration(),
                    User = user.ToDto(),
                    IsNewUser = isNewUser
                });
            }
            catch (InvalidJwtException ex)
            {
                logger.LogWarning(ex, "Invalid Google ID token");
                return Result.Failure<AuthenticationResult>("Invalid Google authentication token");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error authenticating Google user");
                return Result.Failure<AuthenticationResult>("An error occurred during authentication");
            }
        }

        public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var externalLogin = await userRepository.GetExternalLoginByRefreshTokenAsync(refreshToken);

                if (externalLogin == null ||
                    externalLogin.RefreshTokenExpiresAt <= DateTime.UtcNow)
                {
                    return Result.Failure<AuthenticationResult>("Invalid or expired refresh token");
                }

                // Generate new tokens
                var newRefreshToken = tokenService.GenerateRefreshToken();
                externalLogin.UpdateLoginInfo(
                    externalLogin.Name,
                    externalLogin.PictureUrl,
                    newRefreshToken,
                    DateTime.UtcNow.AddDays(30));

                await userRepository.UpdateExternalLoginAsync(externalLogin);

                return Result.Success(new AuthenticationResult
                {
                    AccessToken = tokenService.GenerateAccessToken(externalLogin.User),
                    RefreshToken = newRefreshToken,
                    ExpiresAt = tokenService.GetAccessTokenExpiration(),
                    User = externalLogin.User.ToDto(),
                    IsNewUser = false
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing token");
                return Result.Failure<AuthenticationResult>("An error occurred while refreshing the token");
            }
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var externalLogin = await userRepository.GetExternalLoginByRefreshTokenAsync(refreshToken);

                if (externalLogin == null)
                {
                    return Result.Success(); // Token already revoked or doesn't exist
                }

                externalLogin.UpdateLoginInfo(
                    externalLogin.Name,
                    externalLogin.PictureUrl,
                    null,
                    null);

                await userRepository.UpdateExternalLoginAsync(externalLogin);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error revoking token");
                return Result.Failure("An error occurred while revoking the token");
            }
        }

        public async Task<Result> LinkGoogleAccountAsync(string userId, string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleSettings.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check if this Google account is already linked to another user
                var existingLogin = await userRepository.GetExternalLoginAsync("Google", payload.Subject);
                if (existingLogin != null)
                {
                    return Result.Failure("This Google account is already linked to another user");
                }

                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                // Create external login
                // Create external login
                var newExternalLogin = ExternalLogin.Create(
                    user,
                    "Google",
                    payload.Subject,
                    payload.Email,
                    payload.Name,
                    payload.Picture,
                    tokenService.GenerateRefreshToken(),
                    DateTime.UtcNow.AddDays(30));

                await userRepository.AddExternalLoginAsync(newExternalLogin);
                return Result.Success();
            }
            catch (InvalidJwtException ex)
            {
                logger.LogWarning(ex, "Invalid Google ID token");
                return Result.Failure("Invalid Google authentication token");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error linking Google account");
                return Result.Failure("An error occurred while linking the Google account");
            }
        }

        public async Task<Result> UnlinkGoogleAccountAsync(string userId)
        {
            try
            {
                var externalLogin = await userRepository.GetExternalLoginByUserIdAsync(userId, "Google");
                if (externalLogin == null)
                {
                    return Result.Failure("No Google account linked to this user");
                }

                await userRepository.RemoveExternalLoginAsync(externalLogin);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error unlinking Google account");
                return Result.Failure("An error occurred while unlinking the Google account");
            }
        }

        public async Task<Result<bool>> RequiresTwoFactorAsync(string userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure<bool>("User not found");
                }

                var isInAdminRole = await userManager.IsInRoleAsync(user, "Admin");
                return Result.Success(user.TwoFactorRequired || isInAdminRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking 2FA requirement");
                return Result.Failure<bool>("Error checking two-factor authentication requirement");
            }
        }

        public async Task<Result<AuthenticationResult>> GenerateAuthTokensAsync(User user)
        {
            try
            {
                var refreshToken = tokenService.GenerateRefreshToken();
                var externalLogin = await userRepository.GetExternalLoginByUserIdAsync(user.Id, "Google");
                
                if (externalLogin != null)
                {
                    externalLogin.UpdateLoginInfo(
                        externalLogin.Name,
                        externalLogin.PictureUrl,
                        refreshToken,
                        DateTime.UtcNow.AddDays(30));

                    await userRepository.UpdateExternalLoginAsync(externalLogin);
                }

                return Result.Success(new AuthenticationResult
                {
                    AccessToken = tokenService.GenerateAccessToken(user),
                    RefreshToken = refreshToken,
                    ExpiresAt = tokenService.GetAccessTokenExpiration(),
                    User = user.ToDto()
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating auth tokens");
                return Result.Failure<AuthenticationResult>("Error generating authentication tokens");
            }
        }

        public async Task<Result<TwoFactorSetupResult>> SetupTwoFactorAsync(string userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure<TwoFactorSetupResult>("User not found");
                }

                // Generate 2FA key
                var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
                }

                var email = await userManager.GetEmailAsync(user);
                var qrCodeUrl = $"otpauth://totp/Meritocious:{email}?secret={unformattedKey}&issuer=Meritocious";

                user.TwoFactorEnabled = true;
                await userManager.UpdateAsync(user);

                return Result.Success(new TwoFactorSetupResult
                {
                    SharedKey = unformattedKey,
                    QrCodeUrl = qrCodeUrl
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error setting up 2FA");
                return Result.Failure<TwoFactorSetupResult>("Error setting up two-factor authentication");
            }
        }

        public async Task<Result<bool>> ValidateTwoFactorCodeAsync(string userId, string code)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure<bool>("User not found");
                }

                bool isValid = await userManager.VerifyTwoFactorTokenAsync(
                    user,
                    userManager.Options.Tokens.AuthenticatorTokenProvider,
                    code);

                return Result.Success(isValid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating 2FA code");
                return Result.Failure<bool>("Error validating two-factor authentication code");
            }
        }
    }

    public class GoogleAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}