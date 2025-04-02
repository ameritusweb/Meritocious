using Meritocious.Common.DTOs.Moderation;

namespace Meritocious.Blazor.Services.Api;

public interface IModerationApiService
{
    Task<List<ModerationActionDto>> GetContentModerationHistoryAsync(string contentType, Guid contentId);
    Task<ModerationActionDto> ModerateContentAsync(ModerateContentRequest request);
    Task<ModerationActionDto> AppealModerationAsync(Guid moderationId, AppealModerationRequest request);
    Task<List<ModerationHistoryDto>> GetUserModerationHistoryAsync(Guid userId);
    Task<ModerationActionEffectDto> GetModerationEffectAsync(Guid moderationId);
}

public class ModerationApiService : IModerationApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<ModerationApiService> _logger;

    public ModerationApiService(
        ApiClient apiClient,
        ILogger<ModerationApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<ModerationActionDto>> GetContentModerationHistoryAsync(
        string contentType,
        Guid contentId)
    {
        try
        {
            return await _apiClient.GetAsync<List<ModerationActionDto>>(
                $"api/moderation/{contentType}/{contentId}/history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting moderation history for {ContentType} {ContentId}",
                contentType, contentId);
            throw;
        }
    }

    public async Task<ModerationActionDto> ModerateContentAsync(ModerateContentRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<ModerationActionDto>("api/moderation/actions", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating content");
            throw;
        }
    }

    public async Task<ModerationActionDto> AppealModerationAsync(
        Guid moderationId,
        AppealModerationRequest request)
    {
        try
        {
            return await _apiClient.PostAsync<ModerationActionDto>(
                $"api/moderation/actions/{moderationId}/appeal", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appealing moderation {ModerationId}", moderationId);
            throw;
        }
    }

    public async Task<List<ModerationHistoryDto>> GetUserModerationHistoryAsync(Guid userId)
    {
        try
        {
            return await _apiClient.GetAsync<List<ModerationHistoryDto>>(
                $"api/moderation/users/{userId}/history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting moderation history for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ModerationActionEffectDto> GetModerationEffectAsync(Guid moderationId)
    {
        try
        {
            return await _apiClient.GetAsync<ModerationActionEffectDto>(
                $"api/moderation/actions/{moderationId}/effect");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting effect for moderation {ModerationId}", moderationId);
            throw;
        }
    }
}

public record ModerateContentRequest
{
    public string ContentType { get; init; }
    public Guid ContentId { get; init; }
    public string Action { get; init; }
    public string Reason { get; init; }
    public string? Notes { get; init; }
}

public record AppealModerationRequest
{
    public string Reason { get; init; }
    public string? Evidence { get; init; }
}