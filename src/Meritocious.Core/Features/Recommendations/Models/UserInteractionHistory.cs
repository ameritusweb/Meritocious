using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Recommendations.Models
{
    public class UserInteractionHistory
    {
        public string ContentId { get; set; }
        public ContentType ContentType { get; set; }
        public string InteractionType { get; set; } // view, like, comment, etc.
        public DateTime Timestamp { get; set; }
        public decimal EngagementLevel { get; set; } // 0 to 1
    }
}
