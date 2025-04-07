using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System.Security.Claims;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Extensions;

namespace Meritocious.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthenticationService authService;
        private readonly ILogger<AuthController> logger;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IUserPreferenceService userPreferenceService;

        public AuthController(
            IAuthenticationService authService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserPreferenceService userPreferenceService,
            ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userPreferenceService = userPreferenceService;
            this.logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { Error = "Invalid email or password" });
            }

            var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Error = "Invalid email or password" });
            }

            // Check if Google account is linked
            var logins = await userManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == "Google"))
            {
                return Ok(new { RequiresGoogleLink = true });
            }

            var authResult = await authService.GenerateAuthTokensAsync(user);
            var twoFactorResult = await authService.RequiresTwoFactorAsync(user.Id);
            return Ok(new LoginResponse
            {
                AccessToken = authResult.Value.AccessToken,
                RefreshToken = authResult.Value.RefreshToken,
                ExpiresAt = authResult.Value.ExpiresAt,
                User = user.ToDto(),
                RequiresTwoFactor = twoFactorResult.Value
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegistrationRequest request)
        {
            // Validate input
            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                return BadRequest(new { Error = "Invalid email format" });
            }

            var passwordValidator = new PasswordValidator<User>();
            var passwordResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);
            if (!passwordResult.Succeeded)
            {
                return BadRequest(new { Error = string.Join(", ", passwordResult.Errors.Select(e => e.Description)) });
            }

            // Start transaction
            using var transaction = await userManager.Database.BeginTransactionAsync();
            try
            {
                // Create user
                var user = new User
                {
                    UserName = request.Username,
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    Bio = request.Bio,
                    AvatarUrl = request.AvatarUrl,
                    EmailConfirmed = true // Since we require Google auth
                };

                var result = await userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Error = string.Join(", ", result.Errors.Select(e => e.Description)) });
                }

                // If Google ID token provided, link the account
                if (!string.IsNullOrEmpty(request.GoogleIdToken))
                {
                    var linkResult = await authService.LinkGoogleAccountAsync(user.Id, request.GoogleIdToken);
                    if (!linkResult.IsSuccess)
                    {
                        // Clean up the created user if Google linking fails
                        await userManager.DeleteAsync(user);
                        return BadRequest(new { Error = "Failed to link Google account: " + linkResult.Error });
                    }
                }
                else
                {
                    // If no Google account linked, return flag to enforce linking
                    return Ok(new LoginResponse { RequiresGoogleLink = true });
                }

                // Save user preferences
                if (request.Topics != null || request.ContentPreferences != null)
                {
                    var pResult = await this.userPreferenceService.UpdatePreferencesAsync(
                        user.Id,
                        request.Topics,
                        request.ContentPreferences);

                    if (!pResult.Success)
                    {
                        logger.LogWarning("Failed to save user preferences during registration: {Error}", pResult.Error);
                    }
                }

                // Commit transaction
                await transaction.CommitAsync();

                var authResult = await authService.GenerateAuthTokensAsync(user);
                var twoFactorResult = await authService.RequiresTwoFactorAsync(user.Id);
                return Ok(new LoginResponse
                {
                    AccessToken = authResult.Value.AccessToken,
                    RefreshToken = authResult.Value.RefreshToken,
                    ExpiresAt = authResult.Value.ExpiresAt,
                    User = user.ToDto(),
                    RequiresTwoFactor = twoFactorResult.Value
                });
            }
            catch (Exception ex)
            {
                // Transaction will automatically roll back
                logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { Error = "An error occurred during registration" });
            }
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResult>> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            var result = await authService.AuthenticateGoogleUserAsync(request.IdToken);
            return HandleResult(result);
        }

        [Authorize(Policy = "NoGoogleRequired")]
        [HttpPost("google/link")]
        public async Task<ActionResult<AuthenticationResult>> LinkGoogleAccount([FromBody] GoogleSignInRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await authService.LinkGoogleAccountAsync(userId, request.IdToken);
            if (!result.IsSuccess)
            {
                return HandleResult(result);
            }

            var user = await userManager.FindByIdAsync(userId);
            var authResult = await authService.GenerateAuthTokensAsync(user);
            var twoFactorResult = await authService.RequiresTwoFactorAsync(userId);

            return Ok(new LoginResponse
            {
                AccessToken = authResult.Value.AccessToken,
                RefreshToken = authResult.Value.RefreshToken,
                ExpiresAt = authResult.Value.ExpiresAt,
                User = user.ToDto(),
                RequiresTwoFactor = twoFactorResult.Value
            });
        }

        [Authorize]
        [HttpPost("google/unlink")]
        public async Task<ActionResult> UnlinkGoogleAccount()
        {
            var userId = GetCurrentUserId();
            var result = await authService.UnlinkGoogleAccountAsync(userId);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticationResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        [HttpPost("revoke")]
        public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RevokeTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Ulid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }

            return userId.ToString();
        }
    }

    public class GoogleSignInRequest
    {
        public string IdToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserProfileDto User { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RequiresGoogleLink { get; set; }
    }

    public class RegistrationRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Topics { get; set; }
        public Dictionary<string, decimal> ContentPreferences { get; set; }
        public string GoogleIdToken { get; set; }
    }
}