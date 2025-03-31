
namespace Meritocious.AI.Moderation.Interfaces
{
    using Meritocious.Common.Enums;
    using Meritocious.Core.Entities;

    public interface IContentModerator
    {
        Task<ModerationAction> EvaluateContentAsync(string content);
        Task<Dictionary<string, decimal>> GetToxicityScoresAsync(string content);
    }
}