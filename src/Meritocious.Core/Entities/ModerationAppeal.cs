using Meritocious.Common.DTOs.Moderation;
using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ModerationAppeal : BaseEntity<ModerationAppeal>
    {
        [ForeignKey("FK_ModerationActionId")]
        public UlidId<ModerationAction> ModerationActionId { get; private set; }
        public ModerationAction ModerationAction { get; private set; }
        [ForeignKey("FK_AppealerId")]
        public UlidId<User> AppealerId { get; private set; }
        public User Appealer { get; private set; }
        public string Reason { get; private set; }
        public string AdditionalContext { get; private set; }
        public AppealStatus Status { get; private set; }
        [ForeignKey("FK_ReviewerId")]
        public UlidId<User>? ReviewerId { get; private set; }
        public User Reviewer { get; private set; }
        public string ReviewerNotes { get; private set; }
        public AppealDecision? Decision { get; private set; }
        public DateTime? ReviewedAt { get; private set; }

        private ModerationAppeal()
        {
        }

        public static ModerationAppeal Create(
            ModerationAction action,
            User appealer,
            string reason,
            string additionalContext)
        {
            return new ModerationAppeal
            {
                ModerationActionId = action.Id,
                ModerationAction = action,
                AppealerId = appealer.Id,
                Appealer = appealer,
                Reason = reason,
                AdditionalContext = additionalContext,
                Status = AppealStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Review(
            User reviewer,
            AppealDecision decision,
            string notes)
        {
            ReviewerId = reviewer.Id;
            Reviewer = reviewer;
            Decision = decision;
            ReviewerNotes = notes;
            Status = AppealStatus.Reviewed;
            ReviewedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
