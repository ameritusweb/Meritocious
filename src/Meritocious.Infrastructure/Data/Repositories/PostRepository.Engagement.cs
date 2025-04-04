using Meritocious.Common.DTOs.Engagement;
using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    public partial class PostRepository
    {
        public async Task<List<Post>> GetByIdsWithEngagementAsync(IEnumerable<Guid> postIds)
        {
            return await dbSet
                .Include(p => p.Engagement)
                .Where(p => postIds.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task RecordEngagementAsync(
            Guid postId,
            string region,
            string platform,
            bool isUnique,
            decimal timeSpentSeconds,
            bool bounced)
        {
            var post = await dbSet
                .Include(p => p.Engagement)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                post.RecordView(region, platform, isUnique, timeSpentSeconds, bounced);
                await context.SaveChangesAsync();
            }
        }

        public async Task RecordInteractionAsync(
            Guid postId,
            string interactionType)
        {
            var post = await dbSet
                .Include(p => p.Engagement)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                switch (interactionType.ToLower())
                {
                    case "like":
                        post.RecordLike();
                        break;
                    case "comment":
                        post.RecordComment();
                        break;
                    case "fork":
                        post.RecordFork();
                        break;
                    case "share":
                        post.RecordShare();
                        break;
                    case "citation":
                        post.RecordCitation();
                        break;
                    case "reference":
                        post.RecordReference();
                        break;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEngagementMetricsAsync(
            Guid postId,
            Action<Post> updateAction)
        {
            var post = await dbSet
                .Include(p => p.Engagement)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                updateAction(post);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RemixEngagementMetricsDto> GetEngagementMetricsAsync(Guid postId)
        {
            var post = await dbSet
                .Include(p => p.Engagement)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Engagement?.ToDto() ?? new RemixEngagementMetricsDto();
        }

        public async Task<Dictionary<Guid, RemixEngagementMetricsDto>> GetBulkEngagementMetricsAsync(
            IEnumerable<Guid> postIds)
        {
            var posts = await dbSet
                .Include(p => p.Engagement)
                .Where(p => postIds.Contains(p.Id))
                .ToListAsync();

            return posts.ToDictionary(
                p => p.Id,
                p => p.Engagement?.ToDto() ?? new RemixEngagementMetricsDto());
        }
    }
}
