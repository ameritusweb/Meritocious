using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Moderation.Models
{
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
