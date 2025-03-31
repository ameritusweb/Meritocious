using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<ContentRecommendation>> GetRecommendationsAsync(
            Guid userId,
            List<UserInteractionHistory> userHistory,
            int count = 10,
            List<string> excludedContentIds = null);
    }

    public class ContentRecommendation
    {
        public Guid ContentId { get; set; }
        public string RecommendationType { get; set; }
        public decimal RelevanceScore { get; set; }
        public string Reason { get; set; }
    }
}
