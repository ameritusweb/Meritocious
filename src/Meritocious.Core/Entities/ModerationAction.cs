using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ModerationAction : BaseEntity
    {
        public Guid ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public Guid ModeratorId { get; private set; }
        public User Moderator { get; private set; }
        public ModerationActionType ActionType { get; private set; }
        public string Reason { get; private set; }
        public Dictionary<string, decimal> ToxicityScores { get; private set; }
        public string AutomatedAnalysis { get; private set; }
        public string ModeratorNotes { get; private set; }
        public bool IsAutomated { get; private set; }
        public ModerationDecisionOutcome Outcome { get; private set; }
        public ModerationSeverity Severity { get; private set; }
        public List<ModerationActionEffect> Effects { get; private set; }
        public Guid? AppealId { get; private set; }
        public DateTime? ReviewedAt { get; private set; }
        public Guid? ReviewedById { get; private set; }
        public User ReviewedBy { get; private set; }
        public string ReviewNotes { get; private set; }

        private ModerationAction()
        {
            ToxicityScores = new Dictionary<string, decimal>();
            Effects = new List<ModerationActionEffect>();
        }

        public static ModerationAction Create(
            Guid contentId,
            ContentType contentType,
            User moderator,
            ModerationActionType actionType,
            string reason,
            Dictionary<string, decimal> toxicityScores,
            string automatedAnalysis,
            string moderatorNotes,
            bool isAutomated,
            ModerationSeverity severity)
        {
            return new ModerationAction
            {
                ContentId = contentId,
                ContentType = contentType,
                ModeratorId = moderator.Id,
                Moderator = moderator,
                ActionType = actionType,
                Reason = reason,
                ToxicityScores = toxicityScores,
                AutomatedAnalysis = automatedAnalysis,
                ModeratorNotes = moderatorNotes,
                IsAutomated = isAutomated,
                Severity = severity,
                Outcome = ModerationDecisionOutcome.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AddEffect(ModerationActionEffect effect)
        {
            Effects.Add(effect);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReviewAction(
            User reviewer,
            ModerationDecisionOutcome outcome,
            string notes)
        {
            ReviewedById = reviewer.Id;
            ReviewedBy = reviewer;
            ReviewedAt = DateTime.UtcNow;
            ReviewNotes = notes;
            Outcome = outcome;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
