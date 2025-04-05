using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    using Meritocious.Common.DTOs.Merit;
    using Meritocious.Common.Enums;

    public interface IMeritScoringService
    {
        Task<MeritScoreDto> CalculateContentScoreAsync(string content, ContentType type, string context = null);
        Task<decimal> CalculateUserMeritScoreAsync(string userId);
        Task<List<MeritScoreDto>> GetContentScoreHistoryAsync(string contentId, ContentType type);
        Task<bool> ValidateContentQualityAsync(string content, ContentType type);
    }
}