using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<Post> GetByIdAsync(string id);
        Task<List<Post>> GetByIdsAsync(IEnumerable<string> ids);
        Task<List<Post>> GetTopPostsAsync(int count = 10);
        Task<List<Post>> GetPostsByUserAsync(string userId);
        Task<List<Post>> GetPostsByTagAsync(string tagName);
        Task<List<Post>> GetForkedPostsAsync(string parentPostId);
        Task<List<Post>> GetRemixSourcesAsync(string remixId);
        Task<List<Post>> GetRemixesAsync(string sourcePostId);
        Task<List<Post>> GetPostsByTopicAsync(string topic, DateTime startTime);
        Task<List<Post>> GetPostsAfterDateAsync(DateTime date);
        Task<IList<Post>> GetPostWithRelations(
            string postId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            decimal? minMeritScore = null,
            decimal? maxMeritScore = null,
            string[] includedRelationTypes = null,
            bool includeQuotes = false,
            CancellationToken cancellationToken = default);
        Task<List<UserInteractionHistory>> GetUserInteractionHistoryAsync(string userId);
        Task<int> GetRelationCountAsync(
            string postId,
            string relationType,
            bool asParent = true,
            CancellationToken cancellationToken = default);

        // Migrated from RemixRepository
        Task<Post> GetByIdWithFullDetailsAsync(string id);
        Task<IEnumerable<Post>> GetUserRemixesAsync(string userId, bool includeDrafts = false);
        Task<IEnumerable<Post>> GetRelatedRemixesAsync(string remixId, int limit = 5);
        Task<IEnumerable<Post>> GetTrendingRemixesAsync(int limit = 10);
        Task<Dictionary<string, int>> GetSourceCountsAsync(IEnumerable<string> postIds);
        Task<bool> HasUserRemixedPostAsync(string userId, string postId);
        Task<IEnumerable<Tag>> GetPopularRemixTagsAsync(int limit = 10);
        Task UpdateSynthesisMapAsync(string postId, string synthesisMap);
        Task AddSourceAsync(string remixId, PostRelation source);
        Task RemoveSourceAsync(string remixId, string sourceId);
        Task UpdateSourceOrderAsync(string remixId, IEnumerable<(string SourceId, int NewOrder)> orderUpdates);

        // Note-related methods
        Task<IEnumerable<Note>> GetNotesByPostIdAsync(string postId);
        Task<IEnumerable<Note>> GetNotesBySourceIdAsync(string sourceId);
        Task<IEnumerable<Note>> GetHighConfidenceNotesAsync(string postId, decimal minConfidence = 0.8m);
        Task<IEnumerable<Note>> GetUnusedSuggestionsAsync(string postId);
        Task MarkNoteAppliedAsync(string noteId, bool isApplied);
        Task<Dictionary<string, int>> GetNoteTypeDistributionAsync(string postId);

        // Migrated from RemixSourceRepository
        Task UpdateQuotesAsync(string relationId, IEnumerable<string> quotes);
        Task UpdateRelevanceScoresAsync(string relationId, Dictionary<string, decimal> scores);
        Task<Dictionary<string, int>> GetRelationshipDistributionAsync(string postId);
        Task<PostEngagement> GetEngagementAsync(string postId);
        Task RecordEngagementMetricsAsync(string postId, string region, string platform, bool isUnique, decimal timeSpentSeconds, bool bounced);
        Task RecordInteractionAsync(string postId, string interactionType);

        Task<List<Post>> GetRecentPostsAsync(int count);
    }
}
