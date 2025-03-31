using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.MeritScoring.Interfaces
{
    using Meritocious.Common.DTOs.Merit;

    public interface IMeritScorer
    {
        Task<MeritScoreDto> ScoreContentAsync(string content, string? context = null);
        Task<bool> ValidateContentAsync(string content, decimal minimumScore = 0.3m);
    }
}