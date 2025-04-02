using Meritocious.Web.Components.Substacks;

namespace Meritocious.Blazor.Services.Substacks
{
    public interface ISubstackService
    {
        /// <summary>
        /// Get trending substacks for the specified period
        /// </summary>
        /// <param name="period">Time period: "day", "week", or "month"</param>
        /// <param name="limit">Maximum number of substacks to return</param>
        /// <returns>A list of trending substacks</returns>
        Task<List<SubstackDto>> GetTrendingSubstacksAsync(string period, int limit = 5);
        
        /// <summary>
        /// Get substacks with various filtering, sorting, and pagination options
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and sorting</param>
        /// <returns>Paginated list of substacks</returns>
        Task<PagedResult<SubstackDto>> GetSubstacksAsync(SubstackQueryParams parameters);
        
        /// <summary>
        /// Get a single substack by its ID
        /// </summary>
        /// <param name="id">The substack ID</param>
        /// <returns>The substack data</returns>
        Task<SubstackDto> GetSubstackByIdAsync(Guid id);
        
        /// <summary>
        /// Get a substack by its URL slug
        /// </summary>
        /// <param name="slug">The substack slug</param>
        /// <returns>The substack data</returns>
        Task<SubstackDto> GetSubstackBySlugAsync(string slug);
        
        /// <summary>
        /// Get substacks the current user is following
        /// </summary>
        /// <returns>List of followed substacks</returns>
        Task<List<SubstackDto>> GetFollowedSubstacksAsync();
        
        /// <summary>
        /// Get substacks created by the current user
        /// </summary>
        /// <returns>List of user-created substacks</returns>
        Task<List<SubstackDto>> GetCreatedSubstacksAsync();
        
        /// <summary>
        /// Follow a substack
        /// </summary>
        /// <param name="substackId">ID of the substack to follow</param>
        /// <returns>Success or failure</returns>
        Task<bool> FollowSubstackAsync(Guid substackId);
        
        /// <summary>
        /// Unfollow a substack
        /// </summary>
        /// <param name="substackId">ID of the substack to unfollow</param>
        /// <returns>Success or failure</returns>
        Task<bool> UnfollowSubstackAsync(Guid substackId);
        
        /// <summary>
        /// Create a new substack
        /// </summary>
        /// <param name="createSubstack">Substack creation data</param>
        /// <returns>The newly created substack</returns>
        Task<SubstackDto> CreateSubstackAsync(CreateSubstackDto createSubstack);
        
        /// <summary>
        /// Get recommended substacks for the current user based on their interests
        /// </summary>
        /// <param name="limit">Maximum number of recommendations to return</param>
        /// <returns>List of recommended substacks</returns>
        Task<List<SubstackDto>> GetRecommendedSubstacksAsync(int limit = 5);
    }
    
    /// <summary>
    /// Parameters for querying substacks with filtering, sorting, and pagination
    /// </summary>
    public class SubstackQueryParams
    {
        /// <summary>
        /// Text search query (searches name, description, etc.)
        /// </summary>
        public string SearchQuery { get; set; } = string.Empty;
        
        /// <summary>
        /// List of topics to filter by
        /// </summary>
        public List<string> Topics { get; set; } = new();
        
        /// <summary>
        /// Sort order: "merit", "trending", "newest", "active"
        /// </summary>
        public string SortBy { get; set; } = "merit";
        
        /// <summary>
        /// Minimum merit score threshold (0.0 to 1.0)
        /// </summary>
        public decimal MeritThreshold { get; set; } = 0;
        
        /// <summary>
        /// Page number for pagination (1-based)
        /// </summary>
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
    
    /// <summary>
    /// Represents a paginated result set
    /// </summary>
    /// <typeparam name="T">Type of items in the result</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Items for the current page
        /// </summary>
        public List<T> Items { get; set; } = new();
        
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int Page { get; set; }
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => Page > 1;
        
        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }
    
    /// <summary>
    /// Data for creating a new substack
    /// </summary>
    public class CreateSubstackDto
    {
        /// <summary>
        /// Substack name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Substack description
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// URL-friendly identifier (optional, generated from name if not provided)
        /// </summary>
        public string? Slug { get; set; }
        
        /// <summary>
        /// Topics/tags for categorization
        /// </summary>
        public List<string> Tags { get; set; } = new();
        
        /// <summary>
        /// URL to avatar image
        /// </summary>
        public string? AvatarUrl { get; set; }
        
        /// <summary>
        /// Whether the substack is private (requires approval to join)
        /// </summary>
        public bool IsPrivate { get; set; }
        
        /// <summary>
        /// Guidelines for posting in this substack
        /// </summary>
        public string? Guidelines { get; set; }
    }
}