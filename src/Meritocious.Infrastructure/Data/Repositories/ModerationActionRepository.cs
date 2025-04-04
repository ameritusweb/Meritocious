using Microsoft.EntityFrameworkCore;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Common.Enums;
using Meritocious.Core.Entities;
using Meritocious.Common.DTOs.Moderation;

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
            return await dbSet
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
            var query = dbSet
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
            return await dbSet
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
            var query = dbSet
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
            return await context.Set<ModerationActionEffect>()
                .Include(e => e.ModerationAction)
                .Where(e =>
                    e.ModerationAction.ContentId == contentId &&
                    e.ModerationAction.ContentType == contentType &&
                    !e.IsReverted &&
                    (!e.ExpiresAt.HasValue || e.ExpiresAt > DateTime.UtcNow))
                .ToListAsync();
        }
    }
}