using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IContentTopicRepository
    {
        Task<List<ContentTopic>> GetContentTopicsAsync(Guid contentId, ContentType contentType);
        Task<List<ContentTopic>> GetTopicsForContentListAsync(List<Guid> contentIds, ContentType contentType);
        Task<List<string>> GetTopTrendingTopicsAsync(int count = 10);
        Task<List<ContentTopic>> GetTopicContentAsync(string topic, int limit, decimal minRelevance);
    }
}
