using System.Net.Http.Json;
using Meritocious.Blazor.Services.Auth;
using Meritocious.Web.Components.Substacks;
using Meritocious.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Meritocious.Blazor.Services.Substacks
{
    public class SubstackService : ISubstackService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<SubstackService> _logger;

        public SubstackService(HttpClient httpClient, IAuthService authService, ILogger<SubstackService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        public async Task<List<SubstackDto>> GetTrendingSubstacksAsync(string period, int limit = 5)
        {
            try
            {
                _logger.LogInformation("Fetching trending substacks with period {Period} and limit {Limit}", period, limit);
                var result = await _httpClient.GetFromJsonAsync<List<SubstackDto>>(
                    $"api/substacks/trending?period={period}&limit={limit}");
                
                if (result == null)
                {
                    _logger.LogWarning("No trending substacks found for period {Period}", period);
                    return new List<SubstackDto>();
                }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching trending substacks");
                return new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching trending substacks");
                return new List<SubstackDto>();
            }
        }

        public async Task<PagedResult<SubstackDto>> GetSubstacksAsync(SubstackQueryParams parameters)
        {
            try
            {
                // Construct query string
                var queryString = $"api/substacks?page={parameters.Page}&pageSize={parameters.PageSize}" +
                                 $"&sortBy={parameters.SortBy}&meritThreshold={parameters.MeritThreshold}";
                
                if (!string.IsNullOrEmpty(parameters.SearchQuery))
                {
                    queryString += $"&search={Uri.EscapeDataString(parameters.SearchQuery)}";
                }
                
                foreach (var topic in parameters.Topics)
                {
                    queryString += $"&topics={Uri.EscapeDataString(topic)}";
                }
                
                return await _httpClient.GetFromJsonAsync<PagedResult<SubstackDto>>(queryString) 
                       ?? new PagedResult<SubstackDto>();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching substacks: {ex.Message}");
                return new PagedResult<SubstackDto>();
            }
        }

        public async Task<SubstackDto> GetSubstackByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching substack with ID {SubstackId}", id);
                var result = await _httpClient.GetFromJsonAsync<SubstackDto>($"api/substacks/{id}");
                
                if (result == null)
                {
                    _logger.LogWarning("Substack not found with ID {SubstackId}", id);
                    throw new ResourceNotFoundException($"Substack not found with ID {id}");
                }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching substack with ID {SubstackId}", id);
                throw;
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching substack with ID {SubstackId}", id);
                throw;
            }
        }

        public async Task<SubstackDto> GetSubstackBySlugAsync(string slug)
        {
            try
            {
                _logger.LogInformation("Fetching substack with slug {Slug}", slug);
                var result = await _httpClient.GetFromJsonAsync<SubstackDto>($"api/substacks/slug/{slug}");
                
                if (result == null)
                {
                    _logger.LogWarning("Substack not found with slug {Slug}", slug);
                    throw new ResourceNotFoundException($"Substack not found with slug {slug}");
                }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching substack with slug {Slug}", slug);
                throw;
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching substack with slug {Slug}", slug);
                throw;
            }
        }

        public async Task<List<SubstackDto>> GetFollowedSubstacksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching followed substacks");
                
                if (!await _authService.IsUserAuthenticated())
                {
                    _logger.LogWarning("Unauthenticated user attempted to fetch followed substacks");
                    throw new UnauthorizedAccessException("User must be authenticated to get followed substacks");
                }
                
                var result = await _httpClient.GetFromJsonAsync<List<SubstackDto>>("api/substacks/following");
                
                if (result == null)
                {
                    _logger.LogInformation("No followed substacks found");
                    return new List<SubstackDto>();
                }
                
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching followed substacks");
                return new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching followed substacks");
                return new List<SubstackDto>();
            }
        }

        public async Task<List<SubstackDto>> GetCreatedSubstacksAsync()
        {
            try
            {
                // Ensure user is authenticated
                if (!await _authService.IsUserAuthenticated())
                {
                    throw new UnauthorizedAccessException("User must be authenticated to get created substacks");
                }
                
                return await _httpClient.GetFromJsonAsync<List<SubstackDto>>("api/substacks/created")
                       ?? new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching created substacks: {ex.Message}");
                return new List<SubstackDto>();
            }
        }

        public async Task<bool> FollowSubstackAsync(Guid substackId)
        {
            try
            {
                _logger.LogInformation("Following substack with ID {SubstackId}", substackId);
                
                if (!await _authService.IsUserAuthenticated())
                {
                    _logger.LogWarning("Unauthenticated user attempted to follow substack {SubstackId}", substackId);
                    throw new UnauthorizedAccessException("User must be authenticated to follow a substack");
                }
                
                var response = await _httpClient.PostAsync($"api/substacks/{substackId}/follow", null);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully followed substack {SubstackId}", substackId);
                    return true;
                }
                
                _logger.LogWarning("Failed to follow substack {SubstackId}. Status code: {StatusCode}", 
                    substackId, (int)response.StatusCode);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while following substack {SubstackId}", substackId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while following substack {SubstackId}", substackId);
                return false;
            }
        }

        public async Task<bool> UnfollowSubstackAsync(Guid substackId)
        {
            try
            {
                // Ensure user is authenticated
                if (!await _authService.IsUserAuthenticated())
                {
                    throw new UnauthorizedAccessException("User must be authenticated to unfollow a substack");
                }
                
                var response = await _httpClient.DeleteAsync($"api/substacks/{substackId}/follow");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error unfollowing substack: {ex.Message}");
                return false;
            }
        }

        public async Task<SubstackDto> CreateSubstackAsync(CreateSubstackDto createSubstack)
        {
            try
            {
                _logger.LogInformation("Creating new substack");
                
                if (!await _authService.IsUserAuthenticated())
                {
                    _logger.LogWarning("Unauthenticated user attempted to create substack");
                    throw new UnauthorizedAccessException("User must be authenticated to create a substack");
                }
                
                var response = await _httpClient.PostAsJsonAsync("api/substacks", createSubstack);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubstackDto>();
                    if (result == null)
                    {
                        _logger.LogError("Failed to deserialize created substack response");
                        throw new InvalidOperationException("Failed to deserialize created substack");
                    }
                    
                    _logger.LogInformation("Successfully created substack with ID {SubstackId}", result.Id);
                    return result;
                }
                
                _logger.LogError("Failed to create substack. Status code: {StatusCode}, Reason: {Reason}", 
                    (int)response.StatusCode, response.ReasonPhrase);
                throw new InvalidOperationException($"Failed to create substack: {response.ReasonPhrase}");
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while creating substack");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating substack");
                throw;
            }
        }

        public async Task<List<SubstackDto>> GetRecommendedSubstacksAsync(int limit = 5)
        {
            try
            {
                _logger.LogInformation("Fetching recommended substacks with limit {Limit}", limit);
                
                if (!await _authService.IsUserAuthenticated())
                {
                    _logger.LogInformation("User not authenticated, returning trending substacks instead");
                    return await GetTrendingSubstacksAsync("week", limit);
                }
                
                var result = await _httpClient.GetFromJsonAsync<List<SubstackDto>>($"api/substacks/recommended?limit={limit}");
                
                if (result == null)
                {
                    _logger.LogInformation("No recommended substacks found");
                    return new List<SubstackDto>();
                }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching recommended substacks");
                return new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching recommended substacks");
                return new List<SubstackDto>();
            }
        }
    }
}