using Meritocious.Common.DTOs.Merit;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class MeritScoreHistoryExtensions
    {
        public static MeritScoreHistoryDto ToDto(this MeritScoreHistory meritScoreHistory)
        {
            return new MeritScoreHistoryDto
            {
                Score = meritScoreHistory.Score,
                Timestamp = meritScoreHistory.Timestamp,
                Reason = meritScoreHistory.Reason
            };
        }
    }
}
