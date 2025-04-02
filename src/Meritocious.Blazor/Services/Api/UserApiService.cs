using Meritocious.Common.DTOs.Auth;
using Meritocious.Common.DTOs.Merit;

namespace Meritocious.Blazor.Services.Api;

public interface IUserApiService
{
    Task<UserProfileDto> GetProfileAsync(Guid userId);
    Task<UserProfileDto> GetCurrentUserProfileAsync();
    Task<List<UserProfileDto>> GetTopContributorsAsync(int count = 10);
    Task<MeritScoreDto> GetUserMeritScoreAsync(Guid userId);
    Task UpdateProfileAsync(UserProfileDto profile);
    Task UpdateSettingsAsync(UserSettingsDto settings);
    Task<List<ContributionSummaryDto>> GetUserContributionsAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<AuthorDetailsDto> GetAuthorDetailsAsync(Guid userId);
    Task FollowUserAsync(Guid userId);
    Task UnfollowUserAsync(Guid userId);
}

public class UserApiService : IUserApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<UserApiService> _logger;

    public UserApiService(
        ApiClient apiClient,
        ILogger<UserApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId)
    {
        try
        {
            // Use /api/users/{id}/profile for other users
            // Use /api/users/me/profile for current user
            var endpoint = userId == Guid.Empty ? 
                "api/users/me/profile" : 
                $"api/users/{userId}/profile";
            
            return await _apiClient.GetAsync<UserProfileDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile for {UserId}", userId);
            throw;
        }
    }

    public async Task<UserProfileDto> GetCurrentUserProfileAsync()
    {
        try
        {
            return await _apiClient.GetAsync<UserProfileDto>("api/users/me/profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user profile");
            throw;
        }
    }

    public async Task<List<UserProfileDto>> GetTopContributorsAsync(int count = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<UserProfileDto>>($"api/users/top-contributors?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top contributors");
            throw;
        }
    }

    public async Task<MeritScoreDto> GetUserMeritScoreAsync(Guid userId)
    {
        try
        {
            return await _apiClient.GetAsync<MeritScoreDto>($"api/users/{userId}/merit-score");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting merit score for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateProfileAsync(UserProfileDto profile)
    {
        try
        {
            await _apiClient.PutAsync<Unit>("api/users/me/profile", profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            throw;
        }
    }

    public async Task UpdateSettingsAsync(UserSettingsDto settings)
    {
        try
        {
            await _apiClient.PutAsync<Unit>("api/users/me/settings", settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user settings");
            throw;
        }
    }

    public async Task<List<ContributionSummaryDto>> GetUserContributionsAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            return await _apiClient.GetAsync<List<ContributionSummaryDto>>(
                $"api/users/{userId}/contributions?page={page}&pageSize={pageSize}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contributions for user {UserId}", userId);
            throw;
        }
    }

    public async Task<AuthorDetailsDto> GetAuthorDetailsAsync(Guid userId)
    {
        try
        {
            return await _apiClient.GetAsync<AuthorDetailsDto>($"api/users/{userId}/author-details");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting author details for user {UserId}", userId);
            throw;
        }
    }

    public async Task FollowUserAsync(Guid userId)
    {
        try
        {
            await _apiClient.PostAsync<Unit>($"api/users/{userId}/follow", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {UserId}", userId);
            throw;
        }
    }

    public async Task UnfollowUserAsync(Guid userId)
    {
        try
        {
            await _apiClient.DeleteAsync<Unit>($"api/users/{userId}/follow");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {UserId}", userId);
            throw;
        }
    }
}