using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;

    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<Tag> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count = 10)
        {
            return await _dbSet
                .Include(t => t.Posts)
                .OrderByDescending(t => t.Posts.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(t => t.Name.Contains(searchTerm) ||
                           t.Description.Contains(searchTerm))
                .OrderByDescending(t => t.Posts.Count)
                .ToListAsync();
        }
    }
}