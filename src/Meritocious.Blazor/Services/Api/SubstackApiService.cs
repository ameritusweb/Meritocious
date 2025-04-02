using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Services.Api;

public interface ISubstackApiService
{
    Task<List<SubstackDto>> GetTrendingSubstacksAsync(string period = "day", int count = 10);
    Task<List<SubstackDto>> GetRecommendedSubstacksAsync(int count = 10);
    Task<List<SubstackDto>> GetSimilarSubstacksAsync(Guid substackId, int count = 10);
    Task<List<SubstackDto>> GetFollowedSubstacksAsync();
    Task<SubstackDto> GetSubstackAsync(string slug);
    Task<SubstackDto> CreateSubstackAsync(CreateSubstackRequest request);
    Task<SubstackDto> UpdateSubstackAsync(string slug, UpdateSubstackRequest request);
    Task FollowSubstackAsync(string slug);
    Task UnfollowSubstackAsync(string slug);
    Task<List<string>> GetAvailableTopicsAsync();
    Task<List<PostSummaryDto>> GetSubstackPostsAsync(string slug, int page = 1, int pageSize = 20);
    Task<SubstackMetricsDto> GetSubstackMetricsAsync(string slug);
}

public class SubstackApiService : ISubstackApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<SubstackApiService> _logger;

    public SubstackApiService(
        ApiClient apiClient,
        ILogger<SubstackApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<SubstackDto>> GetTrendingSubstacksAsync(
        string period = "day",
        int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<SubstackDto>>(
                $"api/substacks/trending?period={period}&count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending substacks");
            throw;
        }
    }

    public async Task<List<SubstackDto>> GetRecommendedSubstacksAsync(int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<SubstackDto>>($"api/substacks/recommended?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommended substacks");
            throw;
        }
    }

    public async Task<List<SubstackDto>> GetSimilarSubstacksAsync(Guid substackId, int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<SubstackDto>>(
                $"api/substacks/{substackId}/similar?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar substacks for {SubstackId}", substackId);
            throw;
        }
    }

    public async Task<List<SubstackDto>> GetFollowedSubstacksAsync()
    {
        try
        {
            return await _apiClient.GetAsync<List<SubstackDto>>("api/substacks/following");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followed substacks");
            throw;
        }
    }

    public async Task<SubstackDto> GetSubstackAsync(string slug)
    {
        try
        {
            return await _apiClient.GetAsync<SubstackDto>($"api/substacks/{slug}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting substack {Slug}", slug);
            throw;
        }
    }

    public async Task<SubstackDto> CreateSubstackAsync(CreateSubstackRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<SubstackDto>("api/substacks", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating substack");
            throw;
        }
    }

    public async Task<SubstackDto> UpdateSubstackAsync(string slug, UpdateSubstackRequest request)
    {
        try
        {
            return await _apiClient.PutAsync<SubstackDto>($"api/substacks/{slug}", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating substack {Slug}", slug);
            throw;
        }
    }

    public async Task<SubstackDto> FollowSubstackAsync(string slug)
    {
        try
        {
            // Return the updated substack state after following
            return await _apiClient.PostAsync<SubstackDto>($"api/substacks/{slug}/follow", new {});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following substack {Slug}", slug);
            throw;
        }
    }

    public async Task UnfollowSubstackAsync(string slug)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/substacks/{slug}/follow");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing substack {Slug}", slug);
            throw;
        }
    }

    public async Task<List<string>> GetAvailableTopicsAsync()
    {
        try
        {
            return await _apiClient.GetAsync<List<string>>("api/substacks/topics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available topics");
            throw;
        }
    }

    public async Task<List<PostSummaryDto>> GetSubstackPostsAsync(
        string slug,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostSummaryDto>>(
                $"api/substacks/{slug}/posts?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for substack {Slug}", slug);
            throw;
        }
    }

    public async Task<SubstackMetricsDto> GetSubstackMetricsAsync(string slug)
    {
        try
        {
            return await _apiClient.GetAsync<SubstackMetricsDto>($"api/substacks/{slug}/metrics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metrics for substack {Slug}", slug);
            throw;
        }
    }
}

public record CreateSubstackRequest
{
    public string Name { get; init; }
    public string Description { get; init; }
    public List<string> Topics { get; init; } = new();
    public bool IsPrivate { get; init; }
}

public record UpdateSubstackRequest
{
    public string Description { get; init; }
    public List<string> Topics { get; init; } = new();
    public bool IsPrivate { get; init; }
}