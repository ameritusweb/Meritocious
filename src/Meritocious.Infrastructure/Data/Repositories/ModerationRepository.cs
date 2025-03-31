using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Moderation.Models;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Common.Enums;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class ModerationActionRepository : GenericRepository<ModerationAction>
    {
        public ModerationActionRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ModerationAction>> GetContentModerationHistoryAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _dbSet
                .Include(a => a.Moderator)
                .Include(a => a.ReviewedBy)
                .Include(a => a.Effects)
                .Where(a => a.ContentId == contentId && a.ContentType == contentType)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationAction>> GetModeratorActionsAsync(
            Guid moderatorId,
            DateTime? since = null)
        {
            var query = _dbSet
                .Include(a => a.Effects)
                .Where(a => a.ModeratorId == moderatorId);

            if (since.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= since.Value);
            }

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationAction>> GetPendingReviewActionsAsync()
        {
            return await _dbSet
                .Include(a => a.Moderator)
                .Include(a => a.Effects)
                .Where(a => a.Outcome == ModerationDecisionOutcome.Pending)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationAction>> GetActionsByTypeAsync(
            ModerationActionType actionType,
            DateTime? since = null)
        {
            var query = _dbSet
                .Include(a => a.Moderator)
                .Include(a => a.Effects)
                .Where(a => a.ActionType == actionType);

            if (since.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= since.Value);
            }

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ModerationActionEffect>> GetActiveEffectsAsync(
            Guid contentId,
            ContentType contentType)
        {
            return await _context.Set<ModerationActionEffect>()
                .Include(e => e.ModerationAction)
                .Where(e =>
                    e.ModerationAction.ContentId == contentId &&
                    e.ModerationAction.ContentType == contentType &&
                    !e.IsReverted &&
                    (!e.ExpiresAt.HasValue || e.ExpiresAt > DateTime.UtcNow))
                .ToListAsync();
        }
    }

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