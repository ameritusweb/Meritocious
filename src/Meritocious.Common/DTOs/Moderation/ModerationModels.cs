﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Moderation
{
    public enum ModerationActionType
    {
        Warning,
        ContentRemoval,
        ContentHide,
        UserSuspension,
        UserBan,
        RequireEdit,
        AddNotice,
        RestrictPrivileges,
        RequireApproval,
        AutomaticFilter,
        RequireReview,
        Delete,
        Hide,
        Flag,
        None,
        TagChange
    }

    public enum ModerationDecisionOutcome
    {
        Pending,
        Upheld,
        Modified,
        Reversed,
        Escalated
    }

    public enum ModerationSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum AppealStatus
    {
        Pending,
        UnderReview,
        Reviewed,
        Escalated
    }

    public enum AppealDecision
    {
        Upheld,
        PartiallyUpheld,
        Overturned,
        RequiresEscalation
    }
}
