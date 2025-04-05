using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;
using Meritocious.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class UserTopicPreferenceRepository : GenericRepository<UserTopicPreference>, IUserTopicPreferenceRepository
    {
        public UserTopicPreferenceRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<UserTopicPreference>> GetUserPreferencesAsync(string userId)
        {
            return await dbSet
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Weight)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersInterestedInTopicAsync(
            string topic,
            int limit = 10,
            decimal minWeight = 0.1m)
        {
            return await dbSet
                .Include(p => p.User)
                .Where(p => p.Topic == topic && p.Weight >= minWeight)
                .Take(limit)
                .Select(p => p.User)
                .ToListAsync();
        }

        public Task<Dictionary<string, decimal>> GetUserTopicWeightsAsync(string userId)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }

        public async Task UpdateUserPreferencesAsync(
            string userId,
            Dictionary<string, decimal> preferences)
        {
            var existing = await dbSet
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
                    var user = await context.Users.FindAsync(userId);
                    var newPref = UserTopicPreference.Create(user, pref.Key, pref.Value);
                    await dbSet.AddAsync(newPref);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}