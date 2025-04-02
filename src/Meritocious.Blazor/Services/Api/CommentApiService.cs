using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Services.Api;

public interface ICommentApiService
{
    Task<CommentDto> GetCommentAsync(Guid commentId);
    Task<List<CommentDto>> GetRepliesAsync(Guid commentId, int page = 1, int pageSize = 20);
    Task<CommentDto> CreateCommentAsync(CreateCommentRequest request);
    Task<CommentDto> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request);
    Task DeleteCommentAsync(Guid commentId);
}

public class CommentApiService : ICommentApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<CommentApiService> _logger;

    public CommentApiService(
        ApiClient apiClient,
        ILogger<CommentApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<CommentDto> GetCommentAsync(Guid commentId)
    {
        try
        {
            return await _apiClient.GetAsync<CommentDto>($"api/comments/{commentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<List<CommentDto>> GetRepliesAsync(
        Guid commentId,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<CommentDto>>(
                $"api/comments/{commentId}/replies?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting replies for comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<CommentDto>("api/comments", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            throw;
        }
    }

    public async Task<CommentDto> UpdateCommentAsync(
        Guid commentId,
        UpdateCommentRequest request)
    {
        try
        {
            return await _apiClient.PutAsync<CommentDto>($"api/comments/{commentId}", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/comments/{commentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            throw;
        }
    }
}

public record CreateCommentRequest
{
    public Guid PostId { get; init; }
    public string Content { get; init; }
    public Guid? ParentId { get; init; }
}

public record UpdateCommentRequest
{
    public string Content { get; init; }
}