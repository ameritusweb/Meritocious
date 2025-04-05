using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Recommendations
{
    public class ContentRecommendation
    {
        public string ContentId { get; set; }
        public string RecommendationType { get; set; }
        public decimal RelevanceScore { get; set; }
        public string Reason { get; set; }
    }
}
