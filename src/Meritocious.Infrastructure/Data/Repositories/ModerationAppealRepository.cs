using Meritocious.Common.DTOs.Moderation;
using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ModerationAppealRepository : GenericRepository<ModerationAppeal>
    {
        public ModerationAppealRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ModerationAppeal>> GetPendingAppealsAsync()
        {
            return await _dbSet
                .Include(a => a.ModerationAction)
                .Include(a => a.Appealer)
                .Where(a => a.Status == AppealStatus.Pending)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationAppeal>> GetUserAppealsAsync(Guid userId)
        {
            return await _dbSet
                .Include(a => a.ModerationAction)
                .Include(a => a.Reviewer)
                .Where(a => a.AppealerId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationAppeal>> GetActionAppealsAsync(Guid moderationActionId)
        {
            return await _dbSet
                .Include(a => a.Appealer)
                .Include(a => a.Reviewer)
                .Where(a => a.ModerationActionId == moderationActionId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasPendingAppealAsync(Guid moderationActionId)
        {
            return await _dbSet
                .AnyAsync(a =>
                    a.ModerationActionId == moderationActionId &&
                    a.Status == AppealStatus.Pending);
        }

        public async Task<Dictionary<ModerationActionType, int>> GetAppealStatisticsAsync(
            DateTime start,
            DateTime end)
        {
            var stats = await _dbSet
                .Include(a => a.ModerationAction)
                .Where(a => a.CreatedAt >= start && a.CreatedAt <= end)
                .GroupBy(a => a.ModerationAction.ActionType)
                .Select(g => new
                {
                    ActionType = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return stats.ToDictionary(s => s.ActionType, s => s.Count);
        }
    }
}
