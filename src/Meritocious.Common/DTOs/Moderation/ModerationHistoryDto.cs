using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Moderation
{
    public class ModerationHistoryDto
    {
        public DateTime Timestamp { get; set; }
        public ModerationActionDto Action { get; set; }
        public string Reason { get; set; }
        public bool IsAutomated { get; set; }
        public string ModeratorUsername { get; set; }
        public decimal? MeritScoreBefore { get; set; }
        public decimal? MeritScoreAfter { get; set; }
    }
}
