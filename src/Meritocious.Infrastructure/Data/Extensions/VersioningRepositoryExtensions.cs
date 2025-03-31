﻿using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Versioning.Models;
using Meritocious.Core.Features.Moderation.Models;
using Meritocious.Core.Features.Merit.Models;
using Meritocious.Core.Features.Tags.Models;
using Meritocious.Common.Enums;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Extensions
{
    public static class VersioningRepositoryExtensions
    {
        public static async Task<List<ContentVersion>> GetUserEditsInPeriodAsync(
            this ContentVersionRepository repository,
            Guid userId,
            DateTime start,
            DateTime end)
        {
            return await repository._dbSet
                .Include(v => v.Editor)
                .Where(v =>
                    v.EditorId == userId &&
                    v.CreatedAt >= start &&
                    v.CreatedAt <= end)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public static async Task<Dictionary<ContentEditType, int>> GetEditTypeDistributionAsync(
            this ContentVersionRepository repository,
            Guid contentId,
            ContentType contentType)
        {
            var distribution = await repository._dbSet
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
            Guid moderatorId)
        {
            var outcomes = await repository._dbSet
                .Where(a =>
                    a.ModeratorId == moderatorId &&
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
            return await repository._dbSet
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

            return await repository._dbSet
                .Include(m => m.User)
                .Where(m => m.CreatedAt >= startDate)
                .OrderByDescending(m => m.OverallMeritScore)
                .Take(count)
                .ToListAsync();
        }

        public static async Task<Dictionary<string, List<ReputationBadge>>> GetUserBadgesByTypeAsync(
            this ReputationBadgeRepository repository,
            Guid userId)
        {
            var badges = await repository._dbSet
                .Where(b =>
                    b.UserId == userId &&
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
            return await repository._dbSet
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
            return await repository._dbSet
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
            Guid rootTagId)
        {
            var hierarchy = new Dictionary<string, List<Tag>>();
            var visited = new HashSet<Guid>();

            async Task BuildHierarchyAsync(Guid tagId, string path)
            {
                if (visited.Contains(tagId))
                    return;

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
            return await repository._dbSet
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