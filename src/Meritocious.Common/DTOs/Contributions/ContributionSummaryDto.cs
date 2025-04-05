using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Contributions
{
    public class ContributionSummaryDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
