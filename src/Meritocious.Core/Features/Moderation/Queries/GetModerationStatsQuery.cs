using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Moderation.Queries
{
    public record GetModerationStatsQuery : IRequest<ModerationStatsDto>
    {
        public string TimeFrame { get; init; } = "day";
    }

    public class ModerationStatsDto
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int ResolvedReports { get; set; }
        public Dictionary<string, int> ReportsByType { get; set; } = new();
        public Dictionary<string, int> ActionsTaken { get; set; } = new();
        public decimal AverageResolutionTime { get; set; }
    }
}
