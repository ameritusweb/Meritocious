using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IUserTopicPreferenceRepository
    {
        Task<List<UserTopicPreference>> GetUserPreferencesAsync(string userId);
        Task<List<User>> GetUsersInterestedInTopicAsync(string topic, int limit = 10, decimal minWeight = 0.1m);
        Task UpdateUserPreferencesAsync(string userId, Dictionary<string, decimal> preferences);
        Task<Dictionary<string, decimal>> GetUserTopicWeightsAsync(string userId);
    }
}
