using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Services.Api;

public interface IPostApiService
{
    Task<PostDto> GetPostAsync(Guid postId);
    Task<List<PostSummaryDto>> GetUserPostsAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<List<PostSummaryDto>> GetRecommendedPostsAsync(int count = 10);
    Task<List<PostSummaryDto>> GetTrendingPostsAsync(string period = "day", int count = 10);
    Task<PostDto> CreatePostAsync(CreatePostRequest request);
    Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostRequest request);
    Task DeletePostAsync(Guid postId);
    Task<PostDto> ForkPostAsync(Guid postId, ForkPostRequest request);
    Task<List<CommentDto>> GetPostCommentsAsync(Guid postId, int page = 1, int pageSize = 20);
    Task<CommentDto> AddCommentAsync(Guid postId, AddCommentRequest request);
    Task<List<PostVersionDto>> GetPostHistoryAsync(Guid postId);
}

public class PostApiService : IPostApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<PostApiService> _logger;

    public PostApiService(
        ApiClient apiClient,
        ILogger<PostApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<PostDto> GetPostAsync(Guid postId)
    {
        try
        {
            return await _apiClient.GetAsync<PostDto>($"api/posts/{postId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post {PostId}", postId);
            throw;
        }
    }

    public async Task<List<PostSummaryDto>> GetUserPostsAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostSummaryDto>>(
                $"api/posts/user/{userId}?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<PostSummaryDto>> GetRecommendedPostsAsync(int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostSummaryDto>>($"api/posts/recommended?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommended posts");
            throw;
        }
    }

    public async Task<List<PostSummaryDto>> GetTrendingPostsAsync(
        string period = "day",
        int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostSummaryDto>>(
                $"api/posts/trending?period={period}&count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending posts");
            throw;
        }
    }

    public async Task<PostDto> CreatePostAsync(CreatePostRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<PostDto>("api/posts", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            throw;
        }
    }

    public async Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostRequest request)
    {
        try
        {
            return await _apiClient.PutAsync<PostDto>($"api/posts/{postId}", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", postId);
            throw;
        }
    }

    public async Task DeletePostAsync(Guid postId)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/posts/{postId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", postId);
            throw;
        }
    }

    public async Task<PostDto> ForkPostAsync(Guid postId, ForkPostRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<PostDto>($"api/posts/{postId}/fork", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forking post {PostId}", postId);
            throw;
        }
    }

    public async Task<List<CommentDto>> GetPostCommentsAsync(
        Guid postId,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<CommentDto>>(
                $"api/posts/{postId}/comments?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
            throw;
        }
    }

    public async Task<CommentDto> AddCommentAsync(Guid postId, AddCommentRequest request)
    {
        try
        {
            // Ensure postId is included in both URL and request body
            var command = new AddCommentCommand
            {
                PostId = postId,
                Content = request.Content,
                ParentId = request.ParentId
            };
            
            return await _apiClient.PostAsync<CommentDto>($"api/posts/{postId}/comments", command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to post {PostId}", postId);
            throw;
        }
    }

    public async Task<List<PostVersionDto>> GetPostHistoryAsync(Guid postId)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostVersionDto>>($"api/posts/{postId}/history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history for post {PostId}", postId);
            throw;
        }
    }
}

public record CreatePostRequest
{
    public string Title { get; init; }
    public string Content { get; init; }
    public List<string> Tags { get; init; } = new();
    public bool IsDraft { get; init; }
}

public record UpdatePostRequest
{
    public string Title { get; init; }
    public string Content { get; init; }
    public List<string> Tags { get; init; } = new();
}

public record ForkPostRequest
{
    public string Title { get; init; }
    public string Description { get; init; }
}

public record AddCommentRequest
{
    public string Content { get; init; }
    public Guid? ParentId { get; init; }
}