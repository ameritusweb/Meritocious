using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    using Meritocious.Core.Entities;
    using Meritocious.Common.DTOs.Auth;

    public interface IUserService
    {
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserProfileAsync(string userId, UserProfileDto profile);
        Task UpdateUserMeritScoreAsync(string userId, decimal newScore);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task<IEnumerable<User>> GetModeratorsAsync();
    }
}