using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Tags.Models;

namespace Meritocious.Blazor.Services.Api;

public interface ITagApiService
{
    Task<List<TagDto>> GetPopularTagsAsync(int count = 20);
    Task<List<TagDto>> GetTrendingTagsAsync(string period = "week", int count = 20);
    Task<List<TagDto>> GetRelatedTagsAsync(string tagName, int count = 10);
    Task<List<TagDto>> GetUserTagsAsync(Guid userId);
    Task<TagDto> GetTagDetailsAsync(string tagName);
    Task<List<TagDto>> SearchTagsAsync(string query);
    Task<List<TagSynonymDto>> GetTagSynonymsAsync(string tagName);
    Task AddTagSynonymAsync(string tagName, string synonymName);
    Task<TagWikiDto> GetTagWikiAsync(string tagName);
    Task UpdateTagWikiAsync(string tagName, UpdateTagWikiRequest request);
    Task<List<TagRelationshipDto>> GetTagRelationshipsAsync(string tagName);
    Task AddTagRelationshipAsync(CreateTagRelationshipRequest request);
    Task RemoveTagRelationshipAsync(string parentTag, string childTag);
    Task<List<PostSummaryDto>> GetTaggedPostsAsync(string tagName, int page = 1, int pageSize = 20);
    Task<TagStatsDto> GetTagStatsAsync(string tagName);
    Task FollowTagAsync(string tagName);
    Task UnfollowTagAsync(string tagName);
    Task<List<TagDto>> GetFollowedTagsAsync();
    Task<List<TagModerationLogDto>> GetTagModerationHistoryAsync(string tagName);
}

public class TagApiService : ITagApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<TagApiService> _logger;

    public TagApiService(
        ApiClient apiClient,
        ILogger<TagApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<TagDto>> GetPopularTagsAsync(int count = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>($"api/tags/popular?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            throw;
        }
    }

    public async Task<List<TagDto>> GetTrendingTagsAsync(string period = "week", int count = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>($"api/tags/trending?period={period}&count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending tags");
            throw;
        }
    }

    public async Task<List<TagDto>> GetRelatedTagsAsync(string tagName, int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>($"api/tags/{tagName}/related?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related tags for {TagName}", tagName);
            throw;
        }
    }

    public async Task<List<TagDto>> GetUserTagsAsync(Guid userId)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>($"api/users/{userId}/tags");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TagDto> GetTagDetailsAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<TagDto>($"api/tags/{tagName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting details for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task<List<TagDto>> SearchTagsAsync(string query)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>($"api/tags/search?q={Uri.EscapeDataString(query)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags with query {Query}", query);
            throw;
        }
    }

    public async Task<List<TagSynonymDto>> GetTagSynonymsAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagSynonymDto>>($"api/tags/{tagName}/synonyms");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting synonyms for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task AddTagSynonymAsync(string tagName, string synonymName)
    {
        try
        {
            await _apiClient.PostAsync<Unit>($"api/tags/{tagName}/synonyms", new { SynonymName = synonymName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding synonym {SynonymName} for tag {TagName}", synonymName, tagName);
            throw;
        }
    }

    public async Task<TagWikiDto> GetTagWikiAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<TagWikiDto>($"api/tags/{tagName}/wiki");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wiki for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task UpdateTagWikiAsync(string tagName, UpdateTagWikiRequest request)
    {
        try
        {
            await _apiClient.PutAsync<Unit>($"api/tags/{tagName}/wiki", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wiki for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task<List<TagRelationshipDto>> GetTagRelationshipsAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagRelationshipDto>>($"api/tags/{tagName}/relationships");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting relationships for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task AddTagRelationshipAsync(CreateTagRelationshipRequest request)
    {
        try
        {
            await _apiClient.PostAsync<Unit>("api/tags/relationships", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding relationship between tags {ParentTag} and {ChildTag}", 
                request.ParentTag, request.ChildTag);
            throw;
        }
    }

    public async Task RemoveTagRelationshipAsync(string parentTag, string childTag)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/tags/relationships/{parentTag}/{childTag}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing relationship between tags {ParentTag} and {ChildTag}", 
                parentTag, childTag);
            throw;
        }
    }

    public async Task<List<PostSummaryDto>> GetTaggedPostsAsync(
        string tagName,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            return await _apiClient.GetAsync<List<PostSummaryDto>>(
                $"api/tags/{tagName}/posts?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task<TagStatsDto> GetTagStatsAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<TagStatsDto>($"api/tags/{tagName}/stats");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for tag {TagName}", tagName);
            throw;
        }
    }

    public async Task FollowTagAsync(string tagName)
    {
        try
        {
            await _apiClient.PostAsync<Unit>($"api/tags/{tagName}/follow", new {});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following tag {TagName}", tagName);
            throw;
        }
    }

    public async Task UnfollowTagAsync(string tagName)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/tags/{tagName}/follow");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing tag {TagName}", tagName);
            throw;
        }
    }

    public async Task<List<TagDto>> GetFollowedTagsAsync()
    {
        try
        {
            return await _apiClient.GetAsync<List<TagDto>>("api/tags/following");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followed tags");
            throw;
        }
    }

    public async Task<List<TagModerationLogDto>> GetTagModerationHistoryAsync(string tagName)
    {
        try
        {
            return await _apiClient.GetAsync<List<TagModerationLogDto>>($"api/tags/{tagName}/moderation-history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting moderation history for tag {TagName}", tagName);
            throw;
        }
    }
}

public record UpdateTagWikiRequest
{
    public string Description { get; init; }
    public string Usage { get; init; }
    public List<string> Examples { get; init; } = new();
    public string ChangeComment { get; init; }
}

public record CreateTagRelationshipRequest
{
    public string ParentTag { get; init; }
    public string ChildTag { get; init; }
    public string RelationType { get; init; } // e.g., "parent", "related", "synonym"
    public string Comment { get; init; }
}

public record TagStatsDto
{
    public string TagName { get; init; }
    public int TotalPosts { get; init; }
    public int TotalSubscribers { get; init; }
    public int PostsLast24Hours { get; init; }
    public int PostsLastWeek { get; init; }
    public int PostsLastMonth { get; init; }
    public decimal AverageMeritScore { get; init; }
    public List<ContributorStatsDto> TopContributors { get; init; } = new();
    public List<TimeSeriesDataPoint> UsageTrend { get; init; } = new();
}

public record ContributorStatsDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string AvatarUrl { get; init; }
    public int PostCount { get; init; }
    public decimal AverageMeritScore { get; init; }
}

public record TimeSeriesDataPoint
{
    public DateTime Date { get; init; }
    public int Value { get; init; }
}