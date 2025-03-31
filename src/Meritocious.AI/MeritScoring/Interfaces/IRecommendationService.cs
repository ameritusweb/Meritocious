using Meritocious.AI.SemanticClustering.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.MeritScoring.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<ContentRecommendation>> GetRecommendationsAsync(
            Guid userId,
            List<UserInteractionHistory> userHistory,
            int count = 10,
            List<string> excludedContentIds = null);
    }
}
