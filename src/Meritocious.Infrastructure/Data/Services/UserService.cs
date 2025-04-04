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
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(UserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            // Validate unique username and email
            if (await _userRepository.GetByUsernameAsync(username) != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            var passwordHash = HashPassword(password);
            var user = User.Create(username, email, passwordHash);

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username)
                ?? throw new KeyNotFoundException("User not found");
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email)
                ?? throw new KeyNotFoundException("User not found");
        }

        public async Task UpdateUserProfileAsync(Guid userId, UserProfileDto profile)
        {
            var user = await GetUserByIdAsync(userId);

            // Update allowed fields
            // Note: Username and email changes might require additional verification
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserMeritScoreAsync(Guid userId, decimal newScore)
        {
            var user = await GetUserByIdAsync(userId);
            user.UpdateMeritScore(newScore);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
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
    }
}