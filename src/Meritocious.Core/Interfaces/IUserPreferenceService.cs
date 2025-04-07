namespace Meritocious.Core.Interfaces
{
    public interface IUserPreferenceService
    {
        Task<Result> UpdatePreferencesAsync(string userId, IEnumerable<string> topics, Dictionary<string, decimal> contentPreferences);
        Task<Dictionary<string, decimal>> GetUserPreferencesAsync(string userId);
    }
}