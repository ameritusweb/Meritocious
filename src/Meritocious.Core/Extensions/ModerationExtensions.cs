using Meritocious.Common.DTOs.Moderation;
using Meritocious.Core.Entities;

namespace Meritocious.Core.Extensions
{
    public static class ModerationExtensions
    {
        public static ModerationActionDto ToDto(this ModerationAction action)
        {
            return new ModerationActionDto
            {
                Id = action.Id,
                ContentId = action.ContentId,
                ContentType = action.ContentType,
                ModeratorId = action.ModeratorId,
                ModeratorUsername = action.Moderator?.UserName ?? "System",
                ActionType = action.ActionType,
                Reason = action.Reason,
                ToxicityScores = action.ToxicityScores,
                AutomatedAnalysis = action.AutomatedAnalysis,
                ModeratorNotes = action.ModeratorNotes,
                IsAutomated = action.IsAutomated,
                Outcome = action.Outcome,
                Severity = action.Severity,
                Effects = action.Effects?.Select(e => e.ToDto()).ToList() ?? new List<ModerationActionEffectDto>(),
                AppealId = action.AppealId,
                ReviewedAt = action.ReviewedAt,
                ReviewedById = action.ReviewedById,
                ReviewerUsername = action.ReviewedBy?.UserName,
                ReviewNotes = action.ReviewNotes,
                CreatedAt = action.CreatedAt
            };
        }

        public static ModerationActionEffectDto ToDto(this ModerationActionEffect effect)
        {
            return new ModerationActionEffectDto
            {
                Id = effect.Id,
                ModerationActionId = effect.ModerationActionId,
                EffectType = effect.EffectType,
                EffectData = effect.EffectData,
                ExpiresAt = effect.ExpiresAt,
                IsReverted = effect.IsReverted,
                RevertedAt = effect.RevertedAt,
                RevertReason = effect.RevertReason
            };
        }
    }
}