using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Merit
{
    public class MeritScoreDto
    {
        public decimal ClarityScore { get; set; }
        public decimal NoveltyScore { get; set; }
        public decimal ContributionScore { get; set; }
        public decimal CivilityScore { get; set; }
        public decimal RelevanceScore { get; set; }
        public decimal FinalScore { get; set; }
        public string ModelVersion { get; set; } = string.Empty;
        public Dictionary<string, string> Explanations { get; set; } = new();
    }
}