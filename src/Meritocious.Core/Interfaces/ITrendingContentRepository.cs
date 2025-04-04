using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface ITrendingContentRepository
    {
        Task<List<TrendingContent>> GetTrendingContentAsync(ContentType? contentType = null, int count = 10);
        Task<List<TrendingContent>> GetTrendingByTopicAsync(string topic, int count = 10);
        Task RecalculateTrendingScoresAsync(TimeSpan windowSize);
    }
}
