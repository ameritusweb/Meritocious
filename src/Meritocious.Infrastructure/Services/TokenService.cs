using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using System.Security.Claims;
using Meritocious.Core.Constants;

namespace Meritocious.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings jwtSettings;
        private readonly ISecretsService secretsService;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            ISecretsService secretsService)
        {
            this.jwtSettings = jwtSettings.Value;
            this.secretsService = secretsService;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("merit_score", user.MeritScore.ToString())
            };

            var secretKey = await secretsService.GetSecretAsync(SecretNames.JwtSecret);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);
        }

        public async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = await secretsService.GetSecretAsync(SecretNames.JwtSecret);
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
    }
}