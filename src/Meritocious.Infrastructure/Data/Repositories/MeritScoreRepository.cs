using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Merit.Models;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Entities;
using Meritocious.Common.Enums;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class MeritScoreHistoryRepository : GenericRepository<MeritScoreHistory>
    {
        public MeritScoreHistoryRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<MeritScoreHistory>> GetContentScoreHistoryAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<MeritScoreHistory> GetLatestScoreAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h => h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.EvaluatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<MeritScoreHistory>> GetRecalculationsAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Where(h =>
                    h.ContentId == contentId &&
                    h.ContentType == contentType &&
                    h.IsRecalculation)
                .OrderByDescending(h => h.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetAverageComponentScoresAsync(
            DateTime start,
            DateTime end)
        {
            var scores = await _dbSet
                .Where(h => h.EvaluatedAt >= start && h.EvaluatedAt <= end)
                .SelectMany(h => h.Components)
                .GroupBy(c => c.Key)
                .Select(g => new
                {
                    Component = g.Key,
                    Average = g.Average(c => c.Value)
                })
                .ToListAsync();

            return scores.ToDictionary(s => s.Component, s => s.Average);
        }
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
            var query = _dbSet
                .Include(m => m.User);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(m => m.CategoryScores.ContainsKey(category));
            }

            return await query
                .OrderByDescending(m => m.OverallMeritScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<UserReputationMetrics>> GetUsersByLevelAsync(ReputationLevel level)
        {
            return await _dbSet
                .Include(m => m.User)
                .Where(m => m.Level == level)
                .OrderByDescending(m => m.OverallMeritScore)
                .ToListAsync();
        }

        public async Task<List<UserReputationMetrics>> GetExpertsInTopicAsync(
            string topic,
            int count = 10)
        {
            return await _dbSet
                .Include(m => m.User)
                .Where(m => m.TopicExpertise.ContainsKey(topic))
                .OrderByDescending(m => m.TopicExpertise[topic])
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<ReputationLevel, int>> GetLevelDistributionAsync()
        {
            var distribution = await _dbSet
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

    public class ReputationSnapshotRepository : GenericRepository<ReputationSnapshot>
    {
        public ReputationSnapshotRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ReputationSnapshot>> GetUserSnapshotsAsync(
            Guid userId,
            string timeFrame,
            DateTime? start = null,
            DateTime? end = null)
        {
            var query = _dbSet
                .Where(s => s.UserId == userId && s.TimeFrame == timeFrame);

            if (start.HasValue)
            {
                query = query.Where(s => s.StartDate >= start.Value);
            }

            if (end.HasValue)
            {
                query = query.Where(s => s.EndDate <= end.Value);
            }

            return await query
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<List<ReputationSnapshot>> GetTimeFrameSnapshotsAsync(
            string timeFrame,
            DateTime startDate,
            DateTime endDate)
        {
            return await _dbSet
                .Include(s => s.User)
                .Where(s =>
                    s.TimeFrame == timeFrame &&
                    s.StartDate >= startDate &&
                    s.EndDate <= endDate)
                .OrderByDescending(s => s.OverallMeritScore)
                .ToListAsync();
        }

        public async Task<ReputationSnapshot> GetLatestSnapshotAsync(
            Guid userId,
            string timeFrame)
        {
            return await _dbSet
                .Where(s => s.UserId == userId && s.TimeFrame == timeFrame)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<string, decimal>> GetMetricTrendsAsync(
            Guid userId,
            string timeFrame,
            DateTime start,
            DateTime end)
        {
            var snapshots = await _dbSet
                .Where(s =>
                    s.UserId == userId &&
                    s.TimeFrame == timeFrame &&
                    s.StartDate >= start &&
                    s.EndDate <= end)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

            // Calculate average change for each metric
            var trends = new Dictionary<string, decimal>();
            if (snapshots.Count > 1)
            {
                var firstSnapshot = snapshots.First();
                var lastSnapshot = snapshots.Last();

                foreach (var metric in firstSnapshot.MetricSnapshots.Keys)
                {
                    if (lastSnapshot.MetricSnapshots.ContainsKey(metric))
                    {
                        var change = (lastSnapshot.MetricSnapshots[metric] - firstSnapshot.MetricSnapshots[metric]) /
                                   firstSnapshot.MetricSnapshots[metric];
                        trends[metric] = change;
                    }
                }
            }

            return trends;
        }
    }

    public class ReputationBadgeRepository : GenericRepository<ReputationBadge>
    {
        public ReputationBadgeRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ReputationBadge>> GetUserBadgesAsync(Guid userId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Level)
                .ThenByDescending(b => b.AwardedAt)
                .ToListAsync();
        }

        public async Task<List<ReputationBadge>> GetBadgesByTypeAsync(
            string badgeType,
            bool awardedOnly = true)
        {
            var query = _dbSet
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
            var query = _dbSet
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
            var query = _dbSet
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
            Guid userId,
            decimal minimumProgress = 0.5m)
        {
            return await _dbSet
                .Where(b =>
                    b.UserId == userId &&
                    !b.AwardedAt.HasValue &&
                    b.Progress.Values.Any(p => p >= minimumProgress))
                .OrderByDescending(b => b.Progress.Values.Max())
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetBadgeDistributionAsync(
            string badgeType = null)
        {
            var query = _dbSet.Where(b => b.AwardedAt.HasValue);

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
            var topTypes = await _dbSet
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