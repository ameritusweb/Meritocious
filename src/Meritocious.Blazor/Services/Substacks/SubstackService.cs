using System.Net.Http.Json;
using Meritocious.Blazor.Services.Auth;
using Meritocious.Web.Components.Substacks;

namespace Meritocious.Blazor.Services.Substacks
{
    public class SubstackService : ISubstackService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public SubstackService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<SubstackDto>> GetTrendingSubstacksAsync(string period, int limit = 5)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<SubstackDto>>(
                    $"api/substacks/trending?period={period}&limit={limit}") 
                    ?? new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching trending substacks: {ex.Message}");
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
                return await _httpClient.GetFromJsonAsync<SubstackDto>($"api/substacks/{id}")
                       ?? throw new Exception("Substack not found");
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching substack by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<SubstackDto> GetSubstackBySlugAsync(string slug)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<SubstackDto>($"api/substacks/slug/{slug}")
                       ?? throw new Exception("Substack not found");
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching substack by slug: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SubstackDto>> GetFollowedSubstacksAsync()
        {
            try
            {
                // Ensure user is authenticated
                if (!await _authService.IsUserAuthenticated())
                {
                    throw new UnauthorizedAccessException("User must be authenticated to get followed substacks");
                }
                
                return await _httpClient.GetFromJsonAsync<List<SubstackDto>>("api/substacks/following")
                       ?? new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching followed substacks: {ex.Message}");
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
                // Ensure user is authenticated
                if (!await _authService.IsUserAuthenticated())
                {
                    throw new UnauthorizedAccessException("User must be authenticated to follow a substack");
                }
                
                var response = await _httpClient.PostAsync($"api/substacks/{substackId}/follow", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error following substack: {ex.Message}");
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
                // Ensure user is authenticated
                if (!await _authService.IsUserAuthenticated())
                {
                    throw new UnauthorizedAccessException("User must be authenticated to create a substack");
                }
                
                var response = await _httpClient.PostAsJsonAsync("api/substacks", createSubstack);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SubstackDto>()
                           ?? throw new Exception("Failed to deserialize created substack");
                }
                
                throw new Exception($"Failed to create substack: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error creating substack: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SubstackDto>> GetRecommendedSubstacksAsync(int limit = 5)
        {
            try
            {
                // For unauthenticated users, return trending instead of personalized recommendations
                if (!await _authService.IsUserAuthenticated())
                {
                    return await GetTrendingSubstacksAsync("week", limit);
                }
                
                return await _httpClient.GetFromJsonAsync<List<SubstackDto>>($"api/substacks/recommended?limit={limit}")
                       ?? new List<SubstackDto>();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling/logging
                Console.WriteLine($"Error fetching recommended substacks: {ex.Message}");
                return new List<SubstackDto>();
            }
        }
    }
}