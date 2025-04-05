using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Tags.Models;
using Meritocious.Common.Enums;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.Core.Features.Versioning;
using Meritocious.Common.DTOs.Moderation;

namespace Meritocious.Infrastructure.Data.Extensions
{
    public static class VersioningRepositoryExtensions
    {
        public static async Task<List<ContentVersion>> GetUserEditsInPeriodAsync(
            this ContentVersionRepository repository,
            string userId,
            DateTime start,
            DateTime end)
        {
            return await repository.DbSet
                .Include(v => v.Editor)
                .Where(v =>
                    v.EditorId == userId.ToString() &&
                    v.CreatedAt >= start &&
                    v.CreatedAt <= end)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public static async Task<Dictionary<ContentEditType, int>> GetEditTypeDistributionAsync(
            this ContentVersionRepository repository,
            string contentId,
            ContentType contentType)
        {
            var distribution = await repository.DbSet
                .Where(v => v.ContentId == contentId && v.ContentType == contentType)
                .GroupBy(v => v.EditType)
                .Select(g => new
                {
                    EditType = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return distribution.ToDictionary(d => d.EditType, d => d.Count);
        }
    }

    public static class ModerationRepositoryExtensions
    {
        public static async Task<Dictionary<ModerationDecisionOutcome, int>> GetModeratorEffectivenessAsync(
            this ModerationActionRepository repository,
            string moderatorId)
        {
            var outcomes = await repository.DbSet
                .Where(a =>
                    a.ModeratorId == moderatorId.ToString() &&
                    a.ReviewedAt.HasValue)
                .GroupBy(a => a.Outcome)
                .Select(g => new
                {
                    Outcome = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return outcomes.ToDictionary(o => o.Outcome, o => o.Count);
        }

        public static async Task<List<ModerationAction>> GetContentWithActiveEffectsAsync(
            this ModerationActionRepository repository)
        {
            return await repository.DbSet
                .Include(a => a.Effects)
                .Where(a => a.Effects.Any(e =>
                    !e.IsReverted &&
                    (!e.ExpiresAt.HasValue || e.ExpiresAt > DateTime.UtcNow)))
                .ToListAsync();
        }
    }

    public static class ReputationRepositoryExtensions
    {
        public static async Task<List<UserReputationMetrics>> GetRisingContributorsAsync(
            this UserReputationRepository repository,
            int count = 10,
            int daysWindow = 30)
        {
            var startDate = DateTime.UtcNow.AddDays(-daysWindow);

            return await repository.DbSet
                .Include(m => m.User)
                .Where(m => m.CreatedAt >= startDate)
                .OrderByDescending(m => m.OverallMeritScore)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<Dictionary<string, List<ReputationBadge>>> GetUserBadgesByTypeAsync(
            this ReputationBadgeRepository repository,
            string userId)
        {
            var badges = await repository.DbSet
                .Where(b =>
                    b.UserId == userId.ToString() &&
                    b.AwardedAt.HasValue)
                .GroupBy(b => b.BadgeType)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.OrderByDescending(b => b.Level).ToList());

            return badges;
        }
    }

    public static class TagRepositoryExtensions
    {
        public static async Task<List<Tag>> GetTagsByMeritThresholdAsync(
            this TagRepository repository,
        decimal minThreshold,
            decimal maxThreshold)
        {
            return await repository.DbSet
                .Where(t =>
                    t.Status == TagStatus.Active &&
                    t.MeritThreshold >= minThreshold &&
                    t.MeritThreshold <= maxThreshold)
                .OrderByDescending(t => t.MeritThreshold)
                .ToListAsync();
        }

        public static async Task<List<Tag>> GetTagsWithoutWikiAsync(
            this TagRepository repository,
            int minimumUseCount = 10)
        {
            return await repository.DbSet
                .Include(t => t.WikiVersions)
                .Where(t =>
                    t.Status == TagStatus.Active &&
                    t.UseCount >= minimumUseCount &&
                    !t.WikiVersions.Any(w => w.IsApproved))
                .OrderByDescending(t => t.UseCount)
                .ToListAsync();
        }

        public static async Task<Dictionary<string, List<Tag>>> GetTagHierarchyAsync(
            this TagRepository repository,
            string rootTagId)
        {
            var hierarchy = new Dictionary<string, List<Tag>>();
            var visited = new HashSet<string>();

            async Task BuildHierarchyAsync(string tagId, string path)
            {
                if (visited.Contains(tagId))
                {
                    return;
                }

                visited.Add(tagId);
                var children = await repository.GetChildTagsAsync(tagId);

                if (children.Any())
                {
                    hierarchy[path] = children;
                    foreach (var child in children)
                    {
                        await BuildHierarchyAsync(child.Id, $"{path}/{child.Name}");
                    }
                }
            }

            var rootTag = await repository.GetByIdAsync(rootTagId);
            if (rootTag != null)
            {
                await BuildHierarchyAsync(rootTagId, rootTag.Name);
            }

            return hierarchy;
        }

        public static async Task<List<TagRelationship>> GetTagNetworkAsync(
            this TagRelationshipRepository repository,
            decimal minStrength = 0.5m)
        {
            return await repository.DbSet
                .Include(r => r.SourceTag)
                .Include(r => r.RelatedTag)
                .Where(r =>
                    r.IsApproved &&
                    r.Strength >= minStrength)
                .OrderByDescending(r => r.Strength)
                .ToListAsync();
        }
    }
}