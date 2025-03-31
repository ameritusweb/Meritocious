using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Extensions
{
    public static class UserRepositoryExtensions
    {
        public static async Task<List<User>> GetTopContributorsAsync(
            this UserRepository repository,
            int count = 10,
            DateTime? startDate = null)
        {
            var query = repository._dbSet
                .Where(u => u.IsActive);

            if (startDate.HasValue)
            {
                // Consider only contributions after startDate
                // This would require a more complex query in a real implementation
                // This is a placeholder version
                query = query.Where(u =>
                    u.Posts.Any(p => p.CreatedAt >= startDate.Value) ||
                    u.Comments.Any(c => c.CreatedAt >= startDate.Value));
            }

            return await query
                .OrderByDescending(u => u.MeritScore)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<List<User>> GetModeratorsAsync(
            this UserRepository repository)
        {
            // In a real implementation, we would have a role system
            // This is a placeholder implementation

            // For now, we'll just get the top users by merit score
            return await repository._dbSet
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.MeritScore)
                .Take(5)
                .ToListAsync();
        }
    }
}