using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;

namespace Meritocious.Infrastructure.Data.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IUserTopicPreferenceRepository _userTopicPreferenceRepository;

        public UserPreferenceService(IUserTopicPreferenceRepository userTopicPreferenceRepository)
        {
            _userTopicPreferenceRepository = userTopicPreferenceRepository;
        }

        public async Task<Result> UpdatePreferencesAsync(string userId, IEnumerable<string> topics, Dictionary<string, decimal> contentPreferences)
        {
            try
            {
                // Convert topics to weighted preferences (default weight 1.0)
                var topicPreferences = (topics ?? Enumerable.Empty<string>())
                    .ToDictionary(t => t, _ => 1.0m);

                // Add content preferences
                if (contentPreferences != null)
                {
                    foreach (var pref in contentPreferences)
                    {
                        topicPreferences[pref.Key] = pref.Value;
                    }
                }

                await _userTopicPreferenceRepository.UpdateUserPreferencesAsync(userId, topicPreferences);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update user preferences: {ex.Message}");
            }
        }

        public async Task<Dictionary<string, decimal>> GetUserPreferencesAsync(string userId)
        {
            return await _userTopicPreferenceRepository.GetUserTopicWeightsAsync(userId);
        }
    }
}