using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class UserRepository
    {
        private readonly MeritociousDbContext _context;

        public UserRepository(MeritociousDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<ExternalLogin> GetExternalLoginAsync(string provider, string providerKey)
        {
            return await _context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.Provider == provider &&
                    e.ProviderKey == providerKey);
        }

        public async Task<ExternalLogin> GetExternalLoginByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.RefreshToken == refreshToken &&
                    e.RefreshTokenExpiresAt > DateTime.UtcNow);
        }

        public async Task<ExternalLogin> GetExternalLoginByUserIdAsync(Guid userId, string provider)
        {
            return await _context.Set<ExternalLogin>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId &&
                    e.Provider == provider);
        }

        public async Task<List<ExternalLogin>> GetExternalLoginsForUserAsync(Guid userId)
        {
            return await _context.Set<ExternalLogin>()
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddExternalLoginAsync(ExternalLogin login)
        {
            await _context.Set<ExternalLogin>().AddAsync(login);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExternalLoginAsync(ExternalLogin login)
        {
            _context.Set<ExternalLogin>().Update(login);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveExternalLoginAsync(ExternalLogin login)
        {
            _context.Set<ExternalLogin>().Remove(login);
            await _context.SaveChangesAsync();
        }

        // Optional: Methods for managing user roles, permissions, etc.
        //public async Task<List<string>> GetUserRolesAsync(Guid userId)
        //{
        //    var user = await _context.Users
        //        .Include(u => u.Roles)
        //        .FirstOrDefaultAsync(u => u.Id == userId);

        //    return user?.Roles.Select(r => r.Name).ToList() ?? new List<string>();
        //}

        public async Task<bool> HasExternalLoginAsync(Guid userId, string provider)
        {
            return await _context.Set<ExternalLogin>()
                .AnyAsync(e => e.UserId == userId && e.Provider == provider);
        }

        public async Task<List<User>> GetTopContributorsAsync(int count, DateTime? since = null)
        {
            var query = _context.Users
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
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}