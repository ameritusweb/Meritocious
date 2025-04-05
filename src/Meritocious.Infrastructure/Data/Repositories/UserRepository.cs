using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);

        Task<ExternalLogin?> GetExternalLoginAsync(string provider, string providerKey);
        Task<ExternalLogin?> GetExternalLoginByRefreshTokenAsync(string refreshToken);
        Task<ExternalLogin?> GetExternalLoginByUserIdAsync(string userId, string provider);
        Task<List<ExternalLogin>> GetExternalLoginsForUserAsync(string userId);

        Task AddAsync(User user);
        Task UpdateAsync(User user);

        Task AddExternalLoginAsync(ExternalLogin login);
        Task UpdateExternalLoginAsync(ExternalLogin login);
        Task RemoveExternalLoginAsync(ExternalLogin login);

        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> HasExternalLoginAsync(string userId, string provider);

        Task<List<User>> GetTopContributorsAsync(int count, DateTime? since = null);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly UserManager<User> userManager;

        public UserRepository(MeritociousDbContext context, UserManager<User> userManager) : base(context)
        {
            this.userManager = userManager;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.Id == id.ToString());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<ExternalLogin> GetExternalLoginAsync(string provider, string providerKey)
        {
            return await context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.Provider == provider &&
                    e.ProviderKey == providerKey);
        }

        public async Task<ExternalLogin> GetExternalLoginByRefreshTokenAsync(string refreshToken)
        {
            return await context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.RefreshToken == refreshToken &&
                    e.RefreshTokenExpiresAt > DateTime.UtcNow);
        }

        public async Task<ExternalLogin> GetExternalLoginByUserIdAsync(string userId, string provider)
        {
            return await context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId.ToString() &&
                    e.Provider == provider);
        }

        public async Task<List<ExternalLogin>> GetExternalLoginsForUserAsync(string userId)
        {
            return await context.Set<ExternalLogin>()
                .Include(e => e.User)
                .Where(e => e.UserId == userId.ToString())
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task AddExternalLoginAsync(ExternalLogin login)
        {
            await context.Set<ExternalLogin>().AddAsync(login);
            await context.SaveChangesAsync();
        }

        public async Task UpdateExternalLoginAsync(ExternalLogin login)
        {
            context.Set<ExternalLogin>().Update(login);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExternalLoginAsync(ExternalLogin login)
        {
            context.Set<ExternalLogin>().Remove(login);
            await context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return user == null
                ? new List<string>()
                : (await userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> HasExternalLoginAsync(string userId, string provider)
        {
            return await context.Set<ExternalLogin>()
                .AnyAsync(e => e.UserId == userId.ToString() && e.Provider == provider);
        }

        public async Task<List<User>> GetTopContributorsAsync(int count, DateTime? since = null)
        {
            var query = context.Users
                .OrderByDescending(u => u.MeritScore);

            if (since.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= since.Value)
                    .OrderByDescending(u => u.MeritScore);
            }

            return await query.Take(count).ToListAsync();
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await context.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await context.Users.AnyAsync(u => u.Email == email);
        }
    }
}