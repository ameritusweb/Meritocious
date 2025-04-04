using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<Post> GetByIdAsync(Guid id);
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
        Task<PostEngagement> GetEngagementAsync(Guid postId);
        Task RecordEngagementMetricsAsync(Guid postId, string region, string platform, bool isUnique, decimal timeSpentSeconds, bool bounced);
        Task RecordInteractionAsync(Guid postId, string interactionType);

        Task<List<Post>> GetRecentPostsAsync(int count);
    }
}
