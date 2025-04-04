using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
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
            var query = dbSet
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
            return await dbSet
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
            return await dbSet
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
            var snapshots = await dbSet
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
}
