using Meritocious.Common.Enums;
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
        public string ContentId { get; set; }
        public ContentType ContentType { get; set; }
        public string Context { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRecalculation { get; set; }
        public string RecalculationReason { get; set; }
        public Dictionary<string, decimal> Components { get; set; } = new();
        public string UserId { get; set; }
        public decimal CurrentScore { get; set; }
        public DateTime LastCalculated { get; set; }
        public List<MeritScoreHistoryDto> ScoreHistory { get; set; }
    }
}