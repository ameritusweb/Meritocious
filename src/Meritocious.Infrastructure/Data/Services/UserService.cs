using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Services
{
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Meritocious.Common.DTOs.Auth;
    using Meritocious.Infrastructure.Data.Repositories;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Extensions.Logging;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserService> logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            // Validate unique username and email
            if (await userRepository.GetByUsernameAsync(username) != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await userRepository.GetByEmailAsync(email) != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            var passwordHash = HashPassword(password);
            var user = User.Create(username, email, passwordHash);

            await userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await userRepository.GetByUsernameAsync(username)
                ?? throw new KeyNotFoundException("User not found");
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetByEmailAsync(email)
                ?? throw new KeyNotFoundException("User not found");
        }

        public async Task UpdateUserProfileAsync(string userId, UserProfileDto profile)
        {
            var user = await GetUserByIdAsync(userId);

            // Update allowed fields
            // Note: Username and email changes might require additional verification
            await userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserMeritScoreAsync(string userId, decimal newScore)
        {
            var user = await GetUserByIdAsync(userId);
            user.UpdateMeritScore(newScore);
            await userRepository.UpdateAsync(user);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var passwordHash = HashPassword(password);
            return user.PasswordHash == passwordHash;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public Task<IEnumerable<User>> GetModeratorsAsync()
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}