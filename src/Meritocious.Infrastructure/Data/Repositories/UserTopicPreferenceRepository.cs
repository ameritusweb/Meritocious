using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class UserTopicPreferenceRepository : GenericRepository<UserTopicPreference>
    {
        public UserTopicPreferenceRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<UserTopicPreference>> GetUserPreferencesAsync(Guid userId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Weight)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersInterestedInTopicAsync(
            string topic,
            decimal minWeight = 0.1m)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.Topic == topic && p.Weight >= minWeight)
                .Select(p => p.User)
                .ToListAsync();
        }

        public async Task UpdateUserPreferencesAsync(
            Guid userId,
            Dictionary<string, decimal> preferences)
        {
            var existing = await _dbSet
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var existingTopics = existing.ToDictionary(p => p.Topic);

            foreach (var pref in preferences)
            {
                if (existingTopics.TryGetValue(pref.Key, out var existingPref))
                {
                    existingPref.UpdateWeight(pref.Value);
                }
                else
                {
                    var user = await _context.Users.FindAsync(userId);
                    var newPref = UserTopicPreference.Create(user, pref.Key, pref.Value);
                    await _dbSet.AddAsync(newPref);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
