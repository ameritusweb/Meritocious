using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Services.Api;

public interface IRemixApiService
{
    Task<RemixDto> CreateRemixAsync(CreateRemixRequest request);
    Task<RemixDto> GetRemixAsync(Guid id);
    Task<RemixDto> UpdateRemixAsync(Guid id, UpdateRemixRequest request);
    Task DeleteRemixAsync(Guid id);
    Task<RemixDto> PublishRemixAsync(Guid id);
    Task<RemixSourceDto> AddSourceAsync(Guid remixId, AddSourceRequest request);
    Task RemoveSourceAsync(Guid remixId, Guid sourceId);
    Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<SourceOrderUpdate> updates);
    Task UpdateSourceRelationshipAsync(Guid remixId, Guid sourceId, string relationship);
    Task AddQuoteToSourceAsync(Guid sourceId, AddQuoteRequest request);
    Task<List<RemixDto>> GetMyRemixesAsync(RemixFilter filter);
    Task<List<RemixDto>> GetRelatedRemixesAsync(Guid remixId, int limit = 5);
    Task<List<RemixDto>> GetTrendingRemixesAsync(int limit = 10);
    Task<RemixAnalytics> GetAnalyticsAsync(Guid remixId);
    Task<List<RemixDto>> SearchRemixesAsync(RemixSearchRequest request);
    Task<List<RemixNoteDto>> GenerateInsightsAsync(Guid remixId);
    Task<List<RemixNoteDto>> GetSuggestionsAsync(Guid remixId);
    Task<RemixScoreResult> GetScoreAsync(Guid remixId);
}

public class RemixApiService : IRemixApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<RemixApiService> _logger;

    public RemixApiService(
        ApiClient apiClient,
        ILogger<RemixApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<RemixDto> CreateRemixAsync(CreateRemixRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<RemixDto>("api/remix", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating remix");
            throw;
        }
    }

    public async Task<RemixDto> GetRemixAsync(Guid id)
    {
        try
        {
            return await _apiClient.GetAsync<RemixDto>($"api/remix/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting remix {RemixId}", id);
            throw;
        }
    }

    public async Task<RemixDto> UpdateRemixAsync(Guid id, UpdateRemixRequest request)
    {
        try
        {
            return await _apiClient.PutAsync<RemixDto>($"api/remix/{id}", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating remix {RemixId}", id);
            throw;
        }
    }

    public async Task DeleteRemixAsync(Guid id)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/remix/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting remix {RemixId}", id);
            throw;
        }
    }

    public async Task<RemixDto> PublishRemixAsync(Guid id)
    {
        try
        {
            return await _apiClient.PostAsync<RemixDto>($"api/remix/{id}/publish", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing remix {RemixId}", id);
            throw;
        }
    }

    public async Task<RemixSourceDto> AddSourceAsync(Guid remixId, AddSourceRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<RemixSourceDto>($"api/remix/{remixId}/sources", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding source to remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task RemoveSourceAsync(Guid remixId, Guid sourceId)
    {
        try
        {
            await _apiClient.DeleteAsync($"api/remix/{remixId}/sources/{sourceId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing source {SourceId} from remix {RemixId}", 
                sourceId, remixId);
            throw;
        }
    }

    public async Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<SourceOrderUpdate> updates)
    {
        try
        {
            await _apiClient.PutAsync<Unit>($"api/remix/{remixId}/sources", 
                new UpdateSourcesRequest { OrderUpdates = updates });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating source order for remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task UpdateSourceRelationshipAsync(Guid remixId, Guid sourceId, string relationship)
    {
        try
        {
            await _apiClient.PutAsync<Unit>($"api/remix/{remixId}/sources",
                new UpdateSourcesRequest 
                { 
                    RelationshipUpdate = new RelationshipUpdate 
                    { 
                        SourceId = sourceId,
                        Relationship = relationship
                    }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating source relationship in remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task AddQuoteToSourceAsync(Guid sourceId, AddQuoteRequest request)
    {
        try
        {
            await _apiClient.PostAsync<Unit>($"api/remix/sources/{sourceId}/quotes", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding quote to source {SourceId}", sourceId);
            throw;
        }
    }

    public async Task<List<RemixDto>> GetMyRemixesAsync(RemixFilter filter)
    {
        try
        {
            var queryString = BuildQueryString(filter);
            return await _apiClient.GetAsync<List<RemixDto>>($"api/remix/my{queryString}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user's remixes");
            throw;
        }
    }

    public async Task<List<RemixDto>> GetRelatedRemixesAsync(Guid remixId, int limit = 5)
    {
        try
        {
            return await _apiClient.GetAsync<List<RemixDto>>($"api/remix/{remixId}/related?limit={limit}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related remixes for {RemixId}", remixId);
            throw;
        }
    }

    public async Task<List<RemixDto>> GetTrendingRemixesAsync(int limit = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<RemixDto>>($"api/remix/trending?limit={limit}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending remixes");
            throw;
        }
    }

    public async Task<RemixAnalytics> GetAnalyticsAsync(Guid remixId)
    {
        try
        {
            return await _apiClient.GetAsync<RemixAnalytics>($"api/remix/{remixId}/analytics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics for remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task<List<RemixDto>> SearchRemixesAsync(RemixSearchRequest request)
    {
        try
        {
            var queryString = BuildQueryString(request);
            return await _apiClient.GetAsync<List<RemixDto>>($"api/remix/search{queryString}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching remixes");
            throw;
        }
    }

    public async Task<List<RemixNoteDto>> GenerateInsightsAsync(Guid remixId)
    {
        try
        {
            return await _apiClient.PostAsync<List<RemixNoteDto>>($"api/remix/{remixId}/insights", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating insights for remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task<List<RemixNoteDto>> GetSuggestionsAsync(Guid remixId)
    {
        try
        {
            return await _apiClient.GetAsync<List<RemixNoteDto>>($"api/remix/{remixId}/suggestions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suggestions for remix {RemixId}", remixId);
            throw;
        }
    }

    public async Task<RemixScoreResult> GetScoreAsync(Guid remixId)
    {
        try
        {
            return await _apiClient.GetAsync<RemixScoreResult>($"api/remix/{remixId}/score");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting score for remix {RemixId}", remixId);
            throw;
        }
    }

    private string BuildQueryString<T>(T request) where T : class
    {
        var properties = request.GetType().GetProperties()
            .Where(p => p.GetValue(request) != null)
            .Select(p => $"{p.Name.ToLowerInvariant()}={Uri.EscapeDataString(p.GetValue(request).ToString())}");

        var queryString = string.Join("&", properties);
        return string.IsNullOrEmpty(queryString) ? "" : $"?{queryString}";
    }
}