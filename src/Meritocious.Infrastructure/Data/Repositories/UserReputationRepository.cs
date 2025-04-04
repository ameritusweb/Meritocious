using Meritocious.Core.Features.Reputation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public interface IUserReputationRepository
    {
        Task<List<UserReputationMetrics>> GetTopContributorsAsync(int count = 10, string category = null);
        Task<List<UserReputationMetrics>> GetUsersByLevelAsync(ReputationLevel level);
        Task<List<UserReputationMetrics>> GetExpertsInTopicAsync(string topic, int count = 10);
        Task<Dictionary<ReputationLevel, int>> GetLevelDistributionAsync();
    }

    public class UserReputationRepository : GenericRepository<UserReputationMetrics>
    {
        public UserReputationRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<UserReputationMetrics>> GetTopContributorsAsync(
            int count = 10,
            string category = null)
        {
            var query = dbSet
                .Include(m => m.User);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(m => m.CategoryScores.ContainsKey(category))
                    .Include(m => m.User);
            }

            return await query
                .OrderByDescending(m => m.OverallMeritScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<UserReputationMetrics>> GetUsersByLevelAsync(ReputationLevel level)
        {
            return await dbSet
                .Include(m => m.User)
                .Where(m => m.Level == level)
                .OrderByDescending(m => m.OverallMeritScore)
                .ToListAsync();
        }

        public async Task<List<UserReputationMetrics>> GetExpertsInTopicAsync(
            string topic,
            int count = 10)
        {
            return await dbSet
                .Include(m => m.User)
                .Where(m => m.TopicExpertise.ContainsKey(topic))
                .OrderByDescending(m => m.TopicExpertise[topic])
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<ReputationLevel, int>> GetLevelDistributionAsync()
        {
            var distribution = await dbSet
                .GroupBy(m => m.Level)
                .Select(g => new
                {
                    Level = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return distribution.ToDictionary(d => d.Level, d => d.Count);
        }
    }
}