using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;

    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetTopContributorsAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(u => u.MeritScore)
                .Take(count)
                .ToListAsync();
        }
    }
}