using System;
using Meritocious.Core.Entities;

namespace Meritocious.Core.Entities
{
    public class ExternalLogin : BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public string Provider { get; private set; }
        public string ProviderKey { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string PictureUrl { get; private set; }
        public DateTime LastLoginAt { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiresAt { get; private set; }

        private ExternalLogin() { }

        public static ExternalLogin Create(
            User user,
            string provider,
            string providerKey,
            string email,
            string name,
            string pictureUrl = null,
            string refreshToken = null,
            DateTime? refreshTokenExpiresAt = null)
        {
            return new ExternalLogin
            {
                UserId = user.Id,
                User = user,
                Provider = provider,
                ProviderKey = providerKey,
                Email = email,
                Name = name,
                PictureUrl = pictureUrl,
                LastLoginAt = DateTime.UtcNow,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateLoginInfo(
            string name,
            string pictureUrl,
            string refreshToken = null,
            DateTime? refreshTokenExpiresAt = null)
        {
            Name = name;
            PictureUrl = pictureUrl;
            LastLoginAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(refreshToken))
            {
                RefreshToken = refreshToken;
                RefreshTokenExpiresAt = refreshTokenExpiresAt;
            }

            UpdatedAt = DateTime.UtcNow;
        }
    }
}