using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;

    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<Post>> GetTopPostsAsync(int count = 10)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(p => p.Tags)
                .Where(p => p.AuthorId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByTagAsync(string tagName)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Name == tagName) && !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetForkedPostsAsync(Guid parentPostId)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.ParentRelations)
                .Where(p => p.ParentRelations.Any(r => 
                    r.ParentId == parentPostId && 
                    r.RelationType == "fork") && 
                    !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetRemixSourcesAsync(Guid remixId)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.ChildRelations)
                .Where(p => p.ChildRelations.Any(r => 
                    r.ChildId == remixId && 
                    r.RelationType == "remix"))
                .OrderBy(p => p.ChildRelations
                    .First(r => r.ChildId == remixId).OrderIndex)
                .ToListAsync();
        }

        public async Task<List<Post>> GetRemixesAsync(Guid sourcePostId)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.ParentRelations)
                .Where(p => p.ParentRelations.Any(r => 
                    r.ParentId == sourcePostId && 
                    r.RelationType == "remix"))
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByTopicAsync(string topic, DateTime startTime)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Name == topic) &&
                           p.CreatedAt >= startTime &&
                           !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsAfterDateAsync(DateTime date)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.CreatedAt >= date && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(Guid userId)
        {
            var interactions = await _context.UserContentInteractions
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InteractedAt)
                .Take(100)  // Limit to recent history
                .ToListAsync();

            return interactions.Select(i => new UserInteractionHistory
            {
                ContentId = i.ContentId,
                ContentType = i.ContentType,
                InteractionType = i.InteractionType,
                Timestamp = i.InteractedAt,
                EngagementLevel = i.EngagementScore
            }).ToList();
        }

        public async Task<IList<Post>> GetPostWithRelations(
            Guid postId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            decimal? minMeritScore = null,
            decimal? maxMeritScore = null,
            string[] includedRelationTypes = null,
            bool includeQuotes = false,
            CancellationToken cancellationToken = default)
        {
            // Build base query with relationships
            var query = _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.ParentRelations
                    .Where(r => includedRelationTypes == null || includedRelationTypes.Contains(r.RelationType)))
                .ThenInclude(r => r.Parent)
                .ThenInclude(p => p.Author)
                .Include(p => p.ChildRelations
                    .Where(r => includedRelationTypes == null || includedRelationTypes.Contains(r.RelationType)))
                .ThenInclude(r => r.Child)
                .ThenInclude(p => p.Author)
                .AsQueryable();

            // Build relationship filter
            Expression<Func<Post, bool>> relationFilter = p =>
                p.Id == postId ||
                p.ParentRelations.Any(r => r.ParentId == postId) ||
                p.ChildRelations.Any(r => r.ChildId == postId);

            // Apply filters
            query = query
                .Where(relationFilter)
                .Where(p => !p.IsDeleted)
                .Where(p => !startDate.HasValue || p.CreatedAt >= startDate)
                .Where(p => !endDate.HasValue || p.CreatedAt <= endDate)
                .Where(p => !minMeritScore.HasValue || p.MeritScore >= minMeritScore)
                .Where(p => !maxMeritScore.HasValue || p.MeritScore <= maxMeritScore);

            // Load filtered posts
            var posts = await query.ToListAsync(cancellationToken);

            // For remixes, ensure we have all sources
            var remixPosts = posts.Where(p => 
                p.ParentRelations.Any(r => r.RelationType == "remix") ||
                p.ChildRelations.Any(r => r.RelationType == "remix")).ToList();

            if (remixPosts.Any())
            {
                // Get all related post IDs
                var relatedIds = remixPosts.SelectMany(p => 
                    p.ParentRelations.Select(r => r.ParentId)
                    .Concat(p.ChildRelations.Select(r => r.ChildId)))
                    .Distinct()
                    .Except(posts.Select(p => p.Id));

                if (relatedIds.Any())
                {
                    // Load missing posts
                    var missingPosts = await _dbSet
                        .Include(p => p.Author)
                        .Include(p => p.Tags)
                        .Where(p => relatedIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    posts.AddRange(missingPosts);
                }
            }

            // If requested, load quotes for remix relations
            if (includeQuotes)
            {
                var remixRelations = posts
                    .SelectMany(p => p.ParentRelations.Concat(p.ChildRelations))
                    .Where(r => r.RelationType == "remix")
                    .ToList();

                if (remixRelations.Any())
                {
                    // Load quotes for remix relations
                    await _context.Entry(remixRelations)
                        .Collection(r => r.Quotes)
                        .LoadAsync(cancellationToken);
                }
            }

            return posts;
        }

        public async Task<int> GetRelationCountAsync(
            Guid postId, 
            string relationType,
            bool asParent = true,
            CancellationToken cancellationToken = default)
        {
            if (asParent)
            {
                return await _dbSet
                    .CountAsync(p => p.ParentRelations
                        .Any(r => r.ParentId == postId && 
                             r.RelationType == relationType), 
                        cancellationToken);
            }
            else
            {
                return await _dbSet
                    .CountAsync(p => p.ChildRelations
                        .Any(r => r.ChildId == postId && 
                             r.RelationType == relationType), 
                        cancellationToken);
            }
        }
    }
}
