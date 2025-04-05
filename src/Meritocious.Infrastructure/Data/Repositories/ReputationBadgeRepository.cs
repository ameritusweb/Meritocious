using Meritocious.Core.Entities;
using Meritocious.Core.Features.Versioning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ReputationBadgeRepository : GenericRepository<ReputationBadge>
    {
        public ReputationBadgeRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ReputationBadge>> GetUserBadgesAsync(string userId)
        {
            return await dbSet
                .Include(b => b.User)
                .Where(b => b.UserId == userId.ToString())
                .OrderByDescending(b => b.Level)
                .ThenByDescending(b => b.AwardedAt)
                .ToListAsync();
        }

        public async Task<List<ReputationBadge>> GetBadgesByTypeAsync(
            string badgeType,
            bool awardedOnly = true)
        {
            var query = dbSet
                .Include(b => b.User)
                .Where(b => b.BadgeType == badgeType);

            if (awardedOnly)
            {
                query = query.Where(b => b.AwardedAt.HasValue);
            }

            return await query
                .OrderByDescending(b => b.Level)
                .ThenByDescending(b => b.AwardedAt)
                .ToListAsync();
        }

        public async Task<List<ReputationBadge>> GetBadgesByCategoryAsync(
            string category,
            bool awardedOnly = true)
        {
            var query = dbSet
                .Include(b => b.User)
                .Where(b => b.Category == category);

            if (awardedOnly)
            {
                query = query.Where(b => b.AwardedAt.HasValue);
            }

            return await query
                .OrderByDescending(b => b.Level)
                .ThenByDescending(b => b.AwardedAt)
                .ToListAsync();
        }

        public async Task<List<ReputationBadge>> GetRecentlyAwardedBadgesAsync(
            int count = 10,
            DateTime? since = null)
        {
            var query = dbSet
                .Include(b => b.User)
                .Where(b => b.AwardedAt.HasValue);

            if (since.HasValue)
            {
                query = query.Where(b => b.AwardedAt >= since.Value);
            }

            return await query
                .OrderByDescending(b => b.AwardedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ReputationBadge>> GetProgressingBadgesAsync(
            string userId,
            decimal minimumProgress = 0.5m)
        {
            return await dbSet
                .Where(b =>
                    b.UserId == userId.ToString() &&
                    !b.AwardedAt.HasValue &&
                    b.Progress.Values.Any(p => p >= minimumProgress))
                .OrderByDescending(b => b.Progress.Values.Max())
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetBadgeDistributionAsync(
            string badgeType = null)
        {
            var query = dbSet.Where(b => b.AwardedAt.HasValue);

            if (!string.IsNullOrEmpty(badgeType))
            {
                query = query.Where(b => b.BadgeType == badgeType);
            }

            var distribution = await query
                .GroupBy(b => b.Level)
                .Select(g => new
                {
                    Level = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return distribution.ToDictionary(d => d.Level, d => d.Count);
        }

        public async Task<List<(string BadgeType, int Count)>> GetTopBadgeTypesAsync(int count = 10)
        {
            var topTypes = await dbSet
                .Where(b => b.AwardedAt.HasValue)
                .GroupBy(b => b.BadgeType)
                .Select(g => new
                {
                    BadgeType = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToListAsync();

            return topTypes.Select(t => (t.BadgeType, t.Count)).ToList();
        }
    }
}
