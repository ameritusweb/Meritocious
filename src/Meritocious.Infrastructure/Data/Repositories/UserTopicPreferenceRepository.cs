using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IUserTopicPreferenceRepository
    {
        Task<List<UserTopicPreference>> GetUserPreferencesAsync(Guid userId);
        Task<List<User>> GetUsersInterestedInTopicAsync(string topic, decimal minWeight = 0.1m);
        Task UpdateUserPreferencesAsync(Guid userId, Dictionary<string, decimal> preferences);
    }

    public class UserTopicPreferenceRepository : GenericRepository<UserTopicPreference>
    {
        public UserTopicPreferenceRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<UserTopicPreference>> GetUserPreferencesAsync(Guid userId)
        {
            return await dbSet
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Weight)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersInterestedInTopicAsync(
            string topic,
            decimal minWeight = 0.1m)
        {
            return await dbSet
                .Include(p => p.User)
                .Where(p => p.Topic == topic && p.Weight >= minWeight)
                .Select(p => p.User)
                .ToListAsync();
        }

        public async Task UpdateUserPreferencesAsync(
            Guid userId,
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