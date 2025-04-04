using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;
using Meritocious.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public class TrendingContentRepository : GenericRepository<TrendingContent>, ITrendingContentRepository
    {
        public TrendingContentRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<TrendingContent>> GetTrendingContentAsync(
            ContentType? contentType = null,
            int count = 10)
        {
            var query = dbSet.AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(t => t.ContentType == contentType.Value);
            }

            return await query
                .Where(t => t.WindowEnd >= DateTime.UtcNow)
                .OrderByDescending(t => t.TrendingScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<TrendingContent>> GetTrendingByTopicAsync(
            string topic,
            int count = 10)
        {
            // Join with ContentTopics to find trending content by topic
            var topicContent = await context.Set<ContentTopic>()
                .Where(t => t.Topic == topic)
                .Select(t => t.ContentId)
                .ToListAsync();

            return await dbSet
                .Where(t => topicContent.Contains(t.ContentId) &&
                           t.WindowEnd >= DateTime.UtcNow)
                .OrderByDescending(t => t.TrendingScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task RecalculateTrendingScoresAsync(TimeSpan windowSize)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.Subtract(windowSize);

            // Get all content with interactions in the window
            var interactions = await context.Set<UserContentInteraction>()
                .Where(i => i.InteractedAt >= windowStart)
                .GroupBy(i => new { i.ContentId, i.ContentType })
                .Select(g => new
                {
                    g.Key.ContentId,
                    g.Key.ContentType,
                    ViewCount = g.Count(i => i.InteractionType == "view"),
                    InteractionCount = g.Count(i => i.InteractionType != "view"),
                    AverageMeritScore = g.Average(i => i.EngagementScore)
                })
                .ToListAsync();

            foreach (var interaction in interactions)
            {
                var trending = await dbSet
                    .FirstOrDefaultAsync(t =>
                        t.ContentId == interaction.ContentId &&
                        t.ContentType == interaction.ContentType);

                if (trending == null)
                {
                    trending = TrendingContent.Create(
                        interaction.ContentId,
                        interaction.ContentType,
                        windowSize);
                    await dbSet.AddAsync(trending);
                }

                trending.UpdateMetrics(
                    interaction.ViewCount,
                    interaction.InteractionCount,
                    interaction.AverageMeritScore);
            }

            await context.SaveChangesAsync();
        }
    }
}