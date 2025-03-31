using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Moderation
{
    public class ModerationActionDto
    {
        public Guid Id { get; set; }
        public Guid ContentId { get; set; }
        public ContentType ContentType { get; set; }
        public Guid ModeratorId { get; set; }
        public string ModeratorUsername { get; set; }
        public ModerationActionType ActionType { get; set; }
        public string Reason { get; set; }
        public Dictionary<string, decimal> ToxicityScores { get; set; } = new Dictionary<string, decimal>();
        public string AutomatedAnalysis { get; set; }
        public string ModeratorNotes { get; set; }
        public bool IsAutomated { get; set; }
        public ModerationDecisionOutcome Outcome { get; set; }
        public ModerationSeverity Severity { get; set; }
        public List<ModerationActionEffectDto> Effects { get; set; } = new List<ModerationActionEffectDto>();
        public Guid? AppealId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedById { get; set; }
        public string ReviewerUsername { get; set; }
        public string ReviewNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
