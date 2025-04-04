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

        public async Task<PostEngagement> GetEngagementAsync(Guid postId)
        {
            var post = await GetByIdAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("Post not found", nameof(postId));
            }

            return await GetOrCreateEngagementAsync(post);
        }

        public async Task RecordEngagementMetricsAsync(Guid postId,
            string region,
            string platform,
            bool isUnique,
            decimal timeSpentSeconds,
            bool bounced)
        {
            var post = await GetByIdAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("Post not found", nameof(postId));
            }

            var engagement = await GetOrCreateEngagementAsync(post);
            engagement.RecordView(region, platform, isUnique, timeSpentSeconds, bounced);
            await context.SaveChangesAsync();
        }

        public async Task RecordInteractionAsync(Guid postId, string interactionType)
        {
            var post = await GetByIdAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("Post not found", nameof(postId));
            }

            var engagement = await GetOrCreateEngagementAsync(post);

            switch (interactionType.ToLower())
            {
                case "like":
                    engagement.RecordLike();
                    break;
                case "comment":
                    engagement.RecordComment();
                    break;
                case "fork":
                    engagement.RecordFork();
                    break;
                case "share":
                    engagement.RecordShare();
                    break;
                case "citation":
                    engagement.RecordCitation();
                    break;
                case "reference":
                    engagement.RecordReference();
                    break;
                default:
                    throw new ArgumentException($"Unknown interaction type: {interactionType}", nameof(interactionType));
            }

            await context.SaveChangesAsync();
        }

        private async Task<PostEngagement> GetOrCreateEngagementAsync(Post post)
        {
            var engagement = await context.Set<PostEngagement>()
                .FirstOrDefaultAsync(e => e.PostId == post.Id);

            if (engagement == null)
            {
                engagement = PostEngagement.Create(post);
                await context.Set<PostEngagement>().AddAsync(engagement);
                await context.SaveChangesAsync();
            }

            return engagement;
        }
    }
}
