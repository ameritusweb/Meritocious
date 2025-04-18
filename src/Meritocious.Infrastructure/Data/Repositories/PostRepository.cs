﻿namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;
    using System.Linq.Expressions;
    using Meritocious.Core.Interfaces;

    public partial class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task AddAsync(Post post)
        {
            await dbSet.AddAsync(post);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            dbSet.Update(post);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            post.Delete(); // Soft delete by setting IsDeleted flag
            await context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Post>> GetTopPostsAsync(int count = 10)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserAsync(string userId)
        {
            return await dbSet
                .Include(p => p.Tags)
                .Where(p => p.AuthorId == userId.ToString() && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByTagAsync(string tagName)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Name == tagName) && !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetForkedPostsAsync(string parentPostId)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.ParentRelations)
                .Where(p => p.ParentRelations.Any(r => 
                    r.ParentId == parentPostId && 
                    r.RelationType == "fork") && 
                    !p.IsDeleted)
                .OrderByDescending(p => p.MeritScore)
                .ToListAsync();
        }

        public async Task<List<Post>> GetRemixSourcesAsync(string remixId)
        {
            return await dbSet
                .Include(p => p.Author)
                .Include(p => p.ChildRelations)
                .Where(p => p.ChildRelations.Any(r => 
                    r.ChildId == remixId && 
                    r.RelationType == "remix"))
                .OrderBy(p => p.ChildRelations
                    .First(r => r.ChildId == remixId).OrderIndex)
                .ToListAsync();
        }

        public async Task<List<Post>> GetRemixesAsync(string sourcePostId)
        {
            return await dbSet
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
            return await context.Posts
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
            return await context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => p.CreatedAt >= date && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(string userId)
        {
            var interactions = await context.UserContentInteractions
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
    string postId,
    DateTime? startDate = null,
    DateTime? endDate = null,
    decimal? minMeritScore = null,
    decimal? maxMeritScore = null,
    string[] includedRelationTypes = null,
    bool includeQuotes = false,
    CancellationToken cancellationToken = default)
        {
            // Build base query with relationships
            var query = dbSet
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

            // Filter by relation presence
            Expression<Func<Post, bool>> relationFilter = p =>
                p.Id == postId ||
                p.ParentRelations.Any(r => r.ParentId == postId) ||
                p.ChildRelations.Any(r => r.ChildId == postId);

            // Apply additional filters
            query = query
                .Where(relationFilter)
                .Where(p => !p.IsDeleted)
                .Where(p => !startDate.HasValue || p.CreatedAt >= startDate)
                .Where(p => !endDate.HasValue || p.CreatedAt <= endDate)
                .Where(p => !minMeritScore.HasValue || p.MeritScore >= minMeritScore)
                .Where(p => !maxMeritScore.HasValue || p.MeritScore <= maxMeritScore);

            // Load posts
            var posts = await query.ToListAsync(cancellationToken);

            // Handle remix relations
            var remixPosts = posts.Where(p =>
                p.ParentRelations.Any(r => r.RelationType == "remix") ||
                p.ChildRelations.Any(r => r.RelationType == "remix")).ToList();

            if (remixPosts.Any())
            {
                var relatedIds = remixPosts
                    .SelectMany(p =>
                        p.ParentRelations.Select(r => r.ParentId.Value)
                        .Concat(p.ChildRelations.Select(r => r.ChildId.Value)))
                    .Distinct()
                    .Except(posts.Select(p => p.Id))
                    .ToList();

                if (relatedIds.Any())
                {
                    var missingPosts = await dbSet
                        .Include(p => p.Author)
                        .Include(p => p.Tags)
                        .Where(p => relatedIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    posts.AddRange(missingPosts);
                }
            }

            // Load quotes for remix relations (bulk load)
            if (includeQuotes)
            {
                var remixRelations = posts
                    .SelectMany(p => p.ParentRelations.Concat(p.ChildRelations))
                    .Where(r => r.RelationType == "remix")
                    .ToList();

                if (remixRelations.Any())
                {
                    var remixRelationIds = remixRelations.Select(r => r.Id).ToList();

                    var loadedRemixRelations = await context.PostRelations
                        .Where(r => remixRelationIds.Contains(r.Id))
                        .Include(r => r.Quotes)
                        .ToListAsync(cancellationToken);

                    var quotesByRelationId = loadedRemixRelations.ToDictionary(r => r.Id, r => r.Quotes);

                    foreach (var relation in remixRelations)
                    {
                        if (quotesByRelationId.TryGetValue(relation.Id, out var quotes))
                        {
                            relation.Quotes = quotes;
                        }
                    }
                }
            }

            return posts;
        }

        public async Task<int> GetRelationCountAsync(
            string postId, 
            string relationType,
            bool asParent = true,
            CancellationToken cancellationToken = default)
        {
            if (asParent)
            {
                return await dbSet
                    .CountAsync(p => p.ParentRelations
                        .Any(r => r.ParentId == postId && 
                             r.RelationType == relationType), 
                        cancellationToken);
            }
            else
            {
                return await dbSet
                    .CountAsync(p => p.ChildRelations
                        .Any(r => r.ChildId == postId && 
                             r.RelationType == relationType), 
                        cancellationToken);
            }
        }

        // Migrated from RemixRepository
        public async Task<Post> GetByIdWithFullDetailsAsync(string id)
        {
            return await dbSet
                .Include(p => p.ParentRelations)
                    .ThenInclude(r => r.Parent)
                .Include(p => p.ChildRelations)
                    .ThenInclude(r => r.Child)
                .Include(p => p.Notes)
                .Include(p => p.Tags)
                .Include(p => p.Author)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetUserRemixesAsync(string userId, bool includeDrafts = false)
        {
            var query = dbSet
                .Include(p => p.ParentRelations)
                    .ThenInclude(r => r.Parent)
                .Where(p => p.AuthorId == userId.ToString() && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"));

            if (!includeDrafts)
            {
                query = query.Where(p => !p.IsDraft);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRelatedRemixesAsync(string remixId, int limit = 5)
        {
            // Get tags of the current post
            var currentTags = await dbSet
                .Where(p => p.Id == remixId)
                .SelectMany(p => p.Tags.Select(t => t.Id))
                .ToListAsync();

            // Get posts with similar tags that are remixes
            return await dbSet
                .Where(p => p.Id != remixId && 
                          !p.IsDraft && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"))
                .Include(p => p.Tags)
                .Select(p => new
                {
                    Post = p,
                    TagOverlap = p.Tags.Count(t => currentTags.Contains(t.Id))
                })
                .OrderByDescending(x => x.TagOverlap)
                .ThenByDescending(x => x.Post.MeritScore)
                .Take(limit)
                .Select(x => x.Post)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetTrendingRemixesAsync(int limit = 10)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            return await dbSet
                .Where(p => !p.IsDraft && 
                          p.CreatedAt >= thirtyDaysAgo && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"))
                .OrderByDescending(p => 
                    (p.MeritScore * 0.4m) + 
                    (EF.Functions.DateDiffDay(p.CreatedAt, DateTime.UtcNow) * -0.01m))
                .Take(limit)
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetSourceCountsAsync(IEnumerable<string> postIds)
        {
            var counts = await context.PostRelations
                .Where(r => postIds.Contains(r.ChildId.Value) && r.RelationType == "remix")
                .GroupBy(r => r.ChildId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.PostId.Value, x => x.Count);
        }

        public async Task<bool> HasUserRemixedPostAsync(string userId, string postId)
        {
            return await dbSet
                .AnyAsync(p => p.AuthorId == userId.ToString() && 
                             p.ParentRelations.Any(r => r.ParentId == postId && 
                                                      r.RelationType == "remix"));
        }

        public async Task<IEnumerable<Tag>> GetPopularRemixTagsAsync(int limit = 10)
        {
            return await dbSet
                .Where(p => !p.IsDraft && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"))
                .SelectMany(p => p.Tags)
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(limit)
                .ToListAsync();
        }

        public async Task UpdateSynthesisMapAsync(string postId, string synthesisMap)
        {
            var post = await dbSet.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null)
            {
                post.UpdateSynthesisMap(synthesisMap);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddSourceAsync(string remixId, PostRelation source)
        {
            var post = await dbSet
                .Include(p => p.ParentRelations)
                .FirstOrDefaultAsync(p => p.Id == remixId);

            if (post != null)
            {
                source.OrderIndex = post.ParentRelations.Count;
                post.AddRelation(source);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveSourceAsync(string remixId, string sourceId)
        {
            var relation = await context.PostRelations
                .FirstOrDefaultAsync(r => r.ChildId == remixId && 
                                        r.ParentId == sourceId && 
                                        r.RelationType == "remix");

            if (relation != null)
            {
                context.PostRelations.Remove(relation);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateSourceOrderAsync(string remixId, IEnumerable<(string SourceId, int NewOrder)> orderUpdates)
        {
            var relations = await context.PostRelations
                .Where(r => r.ChildId == remixId && r.RelationType == "remix")
                .ToListAsync();

            foreach (var (sourceId, newOrder) in orderUpdates)
            {
                var relation = relations.FirstOrDefault(r => r.ParentId == sourceId);
                if (relation != null)
                {
                    relation.UpdateOrderIndex(newOrder);
                }
            }

            await context.SaveChangesAsync();
        }

        // Note-related methods
        public async Task<IEnumerable<Note>> GetNotesByPostIdAsync(string postId)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.OrderByDescending(n => n.Confidence) ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetNotesBySourceIdAsync(string sourceId)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Notes.Any(n => n.RelatedSourceIds.Contains(sourceId)));

            return post?.Notes.Where(n => n.RelatedSourceIds.Contains(sourceId))
                            .OrderByDescending(n => n.Confidence) 
                            ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetHighConfidenceNotesAsync(string postId, decimal minConfidence = 0.8m)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.Where(n => n.Confidence >= minConfidence)
                            .OrderByDescending(n => n.Confidence)
                            ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetUnusedSuggestionsAsync(string postId)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.Where(n => !n.IsApplied)
                            .OrderByDescending(n => n.Confidence)
                            ?? Enumerable.Empty<Note>();
        }

        public async Task MarkNoteAppliedAsync(string noteId, bool isApplied)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Notes.Any(n => n.Id == noteId));

            var note = post?.Notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                note.MarkApplied(isApplied);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, int>> GetNoteTypeDistributionAsync(string postId)
        {
            var post = await dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.GroupBy(n => n.Type)
                            .ToDictionary(g => g.Key, g => g.Count())
                            ?? new Dictionary<string, int>();
        }

        // Migrated from RemixSourceRepository
        public async Task UpdateQuotesAsync(string relationId, IEnumerable<string> quotes)
        {
            var relation = await context.PostRelations
                .Include(r => r.Quotes)
                .FirstOrDefaultAsync(r => r.Id == relationId);

            if (relation != null)
            {
                relation.Quotes.Clear();
                foreach (var quote in quotes)
                {
                    relation.AddQuote(new QuoteLocation { Content = quote });
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateRelevanceScoresAsync(string relationId, Dictionary<string, decimal> scores)
        {
            var relation = await context.PostRelations.FirstOrDefaultAsync(r => r.Id == relationId);
            if (relation != null)
            {
                foreach (var (key, score) in scores)
                {
                    relation.UpdateRelevanceScore(score);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, int>> GetRelationshipDistributionAsync(string postId)
        {
            var distribution = await context.PostRelations
                .Where(r => r.ChildId == postId)
                .GroupBy(r => r.RelationType)
                .Select(g => new { RelationType = g.Key, Count = g.Count() })
                .ToListAsync();

            return distribution.ToDictionary(x => x.RelationType, x => x.Count);
        }

        public async Task<List<Post>> GetByIdsWithFullDetailsAsync(string userId, bool includeDrafts = false)
        {
            // Start with base query including all key relationships
            var query = dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.ParentRelations)
                    .ThenInclude(r => r.Parent)
                        .ThenInclude(p => p.Author)
                .Include(p => p.ChildRelations)
                    .ThenInclude(r => r.Child)
                        .ThenInclude(p => p.Author)
                .Include(p => p.Notes)
                .Where(p => p.AuthorId == userId.ToString() && !p.IsDeleted);

            // Apply draft filter
            if (!includeDrafts)
            {
                query = query.Where(p => !p.IsDraft);
            }

            // Get the posts
            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .AsSplitQuery() // Split the query to avoid cartesian explosion
                .ToListAsync();

            // Load quotes for remix relations
            var remixRelations = posts
                .SelectMany(p => p.ParentRelations.Concat(p.ChildRelations))
                .Where(r => r.RelationType == "remix")
                .ToList();

            if (remixRelations.Any())
            {
                var remixRelationIds = remixRelations.Select(r => r.Id).ToList();
                var quotes = await context.Set<QuoteLocation>()
                    .Where(q => remixRelationIds.Contains(q.PostSourceId))
                    .ToListAsync();

                // Group quotes by relation and assign
                var quotesByRelation = quotes.GroupBy(q => q.PostSourceId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var relation in remixRelations)
                {
                    if (quotesByRelation.TryGetValue(relation.Id, out var relationQuotes))
                    {
                        foreach (var quote in relationQuotes)
                        {
                            relation.AddQuote(quote);
                        }
                    }
                }
            }

            // Get engagement metrics if available
            var postIds = posts.Select(p => p.Id).ToList();
            var engagement = await context.Set<PostEngagement>()
                .Where(e => postIds.Contains(e.PostId))
                .ToDictionaryAsync(e => e.PostId);

            foreach (var post in posts)
            {
                await GetOrCreateEngagementAsync(post);
            }

            return posts;
        }

        public async Task<List<Post>> GetByIdsWithFullDetailsAsync(IEnumerable<string> postIds)
        {
            // Similar to above but filtering by postIds instead of userId
            var query = dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.ParentRelations)
                    .ThenInclude(r => r.Parent)
                        .ThenInclude(p => p.Author)
                .Include(p => p.ChildRelations)
                    .ThenInclude(r => r.Child)
                        .ThenInclude(p => p.Author)
                .Include(p => p.Notes)
                .Where(p => postIds.Contains(p.Id) && !p.IsDeleted);

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .AsSplitQuery()
                .ToListAsync();

            // Load quotes and engagement metrics as above
            var remixRelations = posts
                .SelectMany(p => p.ParentRelations.Concat(p.ChildRelations))
                .Where(r => r.RelationType == "remix")
                .ToList();

            if (remixRelations.Any())
            {
                var remixRelationIds = remixRelations.Select(r => r.Id).ToList();
                var quotes = await context.Set<QuoteLocation>()
                    .Where(q => remixRelationIds.Contains(q.PostSourceId))
                    .ToListAsync();

                var quotesByRelation = quotes.GroupBy(q => q.PostSourceId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var relation in remixRelations)
                {
                    if (quotesByRelation.TryGetValue(relation.Id, out var relationQuotes))
                    {
                        foreach (var quote in relationQuotes)
                        {
                            relation.AddQuote(quote);
                        }
                    }
                }
            }

            var engagement = await context.Set<PostEngagement>()
                .Where(e => postIds.Contains(e.PostId))
                .ToDictionaryAsync(e => e.PostId);

            foreach (var post in posts)
            {
                await GetOrCreateEngagementAsync(post);
            }

            return posts;
        }

        public async Task<List<Post>> GetForksFromSourceAsync(
        string sourceUrl,
        CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p => p.ExternalForkSource != null && p.ExternalForkSource.SourceUrl == sourceUrl)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Post>> GetForksByTypeAsync(
            string forkType,
            CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ForkType)
                .Where(p => p.ForkType != null && p.ForkType.Name == forkType)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Post>> GetForksByPlatformAsync(
            string platform,
            CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p => p.ExternalForkSource != null && p.ExternalForkSource.Platform == platform)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Post>> SearchForksByTagsAsync(
            IEnumerable<string> tags,
            CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p =>
                    p.ExternalForkSource != null &&
                    p.ExternalForkSource.Tags.Any(t => tags.Contains(t)))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Post>> GetForksWithQuotesAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p => !string.IsNullOrEmpty(p.ExternalForkQuote))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<string, int>> GetForkCountsByPlatformAsync(
            CancellationToken cancellationToken = default)
        {
            var counts = await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p => p.ExternalForkSource != null)
                .GroupBy(p => p.ExternalForkSource.Platform)
                .Select(g => new { Platform = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return counts.ToDictionary(x => x.Platform, x => x.Count);
        }

        public async Task<List<Post>> GetForksInDateRangeAsync(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.ExternalForkSource)
                .Where(p =>
                    p.ExternalForkSource != null &&
                    p.CreatedAt >= start &&
                    p.CreatedAt <= end)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Post>> GetRecentPostsAsync(int count)
        {
            // TODO: Complete this.
            throw new NotImplementedException();
        }
    }
}
