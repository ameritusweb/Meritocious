namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;
    using System.Linq.Expressions;

    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<List<Post>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<List<Post>> GetTopPostsAsync(int count = 10);
        Task<List<Post>> GetPostsByUserAsync(Guid userId);
        Task<List<Post>> GetPostsByTagAsync(string tagName);
        Task<List<Post>> GetForkedPostsAsync(Guid parentPostId);
        Task<List<Post>> GetRemixSourcesAsync(Guid remixId);
        Task<List<Post>> GetRemixesAsync(Guid sourcePostId);
        Task<List<Post>> GetPostsByTopicAsync(string topic, DateTime startTime);
        Task<List<Post>> GetPostsAfterDateAsync(DateTime date);
        Task<IList<Post>> GetPostWithRelations(
            Guid postId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            decimal? minMeritScore = null,
            decimal? maxMeritScore = null,
            string[] includedRelationTypes = null,
            bool includeQuotes = false,
            CancellationToken cancellationToken = default);
        Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(Guid userId);
        Task<int> GetRelationCountAsync(
            Guid postId,
            string relationType,
            bool asParent = true,
            CancellationToken cancellationToken = default);

        // Migrated from RemixRepository
        Task<Post> GetByIdWithFullDetailsAsync(Guid id);
        Task<IEnumerable<Post>> GetUserRemixesAsync(Guid userId, bool includeDrafts = false);
        Task<IEnumerable<Post>> GetRelatedRemixesAsync(Guid remixId, int limit = 5);
        Task<IEnumerable<Post>> GetTrendingRemixesAsync(int limit = 10);
        Task<Dictionary<Guid, int>> GetSourceCountsAsync(IEnumerable<Guid> postIds);
        Task<bool> HasUserRemixedPostAsync(Guid userId, Guid postId);
        Task<IEnumerable<Tag>> GetPopularRemixTagsAsync(int limit = 10);
        Task UpdateSynthesisMapAsync(Guid postId, string synthesisMap);
        Task AddSourceAsync(Guid remixId, PostRelation source);
        Task RemoveSourceAsync(Guid remixId, Guid sourceId);
        Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<(Guid SourceId, int NewOrder)> orderUpdates);

        // Note-related methods
        Task<IEnumerable<Note>> GetNotesByPostIdAsync(Guid postId);
        Task<IEnumerable<Note>> GetNotesBySourceIdAsync(Guid sourceId);
        Task<IEnumerable<Note>> GetHighConfidenceNotesAsync(Guid postId, decimal minConfidence = 0.8m);
        Task<IEnumerable<Note>> GetUnusedSuggestionsAsync(Guid postId);
        Task MarkNoteAppliedAsync(Guid noteId, bool isApplied);
        Task<Dictionary<string, int>> GetNoteTypeDistributionAsync(Guid postId);

        // Migrated from RemixSourceRepository
        Task UpdateQuotesAsync(Guid relationId, IEnumerable<string> quotes);
        Task UpdateRelevanceScoresAsync(Guid relationId, Dictionary<string, decimal> scores);
        Task<Dictionary<string, int>> GetRelationshipDistributionAsync(Guid postId);
    }

    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Post post)
        {
            await _dbSet.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _dbSet.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            post.Delete(); // Soft delete by setting IsDeleted flag
            await _context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _dbSet
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();
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
                        p.ParentRelations.Select(r => r.ParentId)
                        .Concat(p.ChildRelations.Select(r => r.ChildId)))
                    .Distinct()
                    .Except(posts.Select(p => p.Id))
                    .ToList();

                if (relatedIds.Any())
                {
                    var missingPosts = await _dbSet
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

                    var loadedRemixRelations = await _context.PostRelations
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

        // Migrated from RemixRepository
        public async Task<Post> GetByIdWithFullDetailsAsync(Guid id)
        {
            return await _dbSet
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

        public async Task<IEnumerable<Post>> GetUserRemixesAsync(Guid userId, bool includeDrafts = false)
        {
            var query = _dbSet
                .Include(p => p.ParentRelations)
                    .ThenInclude(r => r.Parent)
                .Where(p => p.AuthorId == userId && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"));

            if (!includeDrafts)
            {
                query = query.Where(p => !p.IsDraft);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRelatedRemixesAsync(Guid remixId, int limit = 5)
        {
            // Get tags of the current post
            var currentTags = await _dbSet
                .Where(p => p.Id == remixId)
                .SelectMany(p => p.Tags.Select(t => t.Id))
                .ToListAsync();

            // Get posts with similar tags that are remixes
            return await _dbSet
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

            return await _dbSet
                .Where(p => !p.IsDraft && 
                          p.CreatedAt >= thirtyDaysAgo && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"))
                .OrderByDescending(p => 
                    (p.MeritScore * 0.4) + 
                    (EF.Functions.DateDiffDay(p.CreatedAt, DateTime.UtcNow) * -0.01))
                .Take(limit)
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetSourceCountsAsync(IEnumerable<Guid> postIds)
        {
            var counts = await _context.PostRelations
                .Where(r => postIds.Contains(r.ChildId) && r.RelationType == "remix")
                .GroupBy(r => r.ChildId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.PostId, x => x.Count);
        }

        public async Task<bool> HasUserRemixedPostAsync(Guid userId, Guid postId)
        {
            return await _dbSet
                .AnyAsync(p => p.AuthorId == userId && 
                             p.ParentRelations.Any(r => r.ParentId == postId && 
                                                      r.RelationType == "remix"));
        }

        public async Task<IEnumerable<Tag>> GetPopularRemixTagsAsync(int limit = 10)
        {
            return await _dbSet
                .Where(p => !p.IsDraft && 
                          p.ParentRelations.Any(r => r.RelationType == "remix"))
                .SelectMany(p => p.Tags)
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(limit)
                .ToListAsync();
        }

        public async Task UpdateSynthesisMapAsync(Guid postId, string synthesisMap)
        {
            var post = await _dbSet.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null)
            {
                post.UpdateSynthesisMap(synthesisMap);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddSourceAsync(Guid remixId, PostRelation source)
        {
            var post = await _dbSet
                .Include(p => p.ParentRelations)
                .FirstOrDefaultAsync(p => p.Id == remixId);

            if (post != null)
            {
                source.OrderIndex = post.ParentRelations.Count;
                post.AddRelation(source);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveSourceAsync(Guid remixId, Guid sourceId)
        {
            var relation = await _context.PostRelations
                .FirstOrDefaultAsync(r => r.ChildId == remixId && 
                                        r.ParentId == sourceId && 
                                        r.RelationType == "remix");

            if (relation != null)
            {
                _context.PostRelations.Remove(relation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<(Guid SourceId, int NewOrder)> orderUpdates)
        {
            var relations = await _context.PostRelations
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

            await _context.SaveChangesAsync();
        }

        // Note-related methods
        public async Task<IEnumerable<Note>> GetNotesByPostIdAsync(Guid postId)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.OrderByDescending(n => n.Confidence) ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetNotesBySourceIdAsync(Guid sourceId)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Notes.Any(n => n.RelatedSourceIds.Contains(sourceId)));

            return post?.Notes.Where(n => n.RelatedSourceIds.Contains(sourceId))
                            .OrderByDescending(n => n.Confidence) 
                            ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetHighConfidenceNotesAsync(Guid postId, decimal minConfidence = 0.8m)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.Where(n => n.Confidence >= minConfidence)
                            .OrderByDescending(n => n.Confidence)
                            ?? Enumerable.Empty<Note>();
        }

        public async Task<IEnumerable<Note>> GetUnusedSuggestionsAsync(Guid postId)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.Where(n => !n.IsApplied)
                            .OrderByDescending(n => n.Confidence)
                            ?? Enumerable.Empty<Note>();
        }

        public async Task MarkNoteAppliedAsync(Guid noteId, bool isApplied)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Notes.Any(n => n.Id == noteId));

            var note = post?.Notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                note.MarkApplied(isApplied);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, int>> GetNoteTypeDistributionAsync(Guid postId)
        {
            var post = await _dbSet
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post?.Notes.GroupBy(n => n.Type)
                            .ToDictionary(g => g.Key, g => g.Count())
                            ?? new Dictionary<string, int>();
        }

        // Migrated from RemixSourceRepository
        public async Task UpdateQuotesAsync(Guid relationId, IEnumerable<string> quotes)
        {
            var relation = await _context.PostRelations
                .Include(r => r.Quotes)
                .FirstOrDefaultAsync(r => r.Id == relationId);

            if (relation != null)
            {
                relation.Quotes.Clear();
                foreach (var quote in quotes)
                {
                    relation.AddQuote(new QuoteLocation { Content = quote });
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRelevanceScoresAsync(Guid relationId, Dictionary<string, decimal> scores)
        {
            var relation = await _context.PostRelations.FirstOrDefaultAsync(r => r.Id == relationId);
            if (relation != null)
            {
                foreach (var (key, score) in scores)
                {
                    relation.UpdateRelevanceScore(score);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, int>> GetRelationshipDistributionAsync(Guid postId)
        {
            var distribution = await _context.PostRelations
                .Where(r => r.ChildId == postId)
                .GroupBy(r => r.RelationType)
                .Select(g => new { RelationType = g.Key, Count = g.Count() })
                .ToListAsync();

            return distribution.ToDictionary(x => x.RelationType, x => x.Count);
        }
    }
}
