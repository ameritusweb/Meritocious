using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface IContentSimilarityRepository
    {
        Task<List<ContentSimilarity>> GetSimilarContentAsync(string contentId, decimal minSimilarity = 0.7m);
        Task<decimal> GetContentSimilarityAsync(string contentId1, string contentId2);
        Task<List<(string id1, string id2)>> GetContentPairsForUpdateAsync(int batchSize = 100);
        Task MarkForUpdateAsync(string contentId, int priority = 0);
        Task MarkOldSimilaritiesForUpdateAsync(TimeSpan age, int priority = 0);
        Task CreateMissingSimilaritiesAsync(List<string> contentIds);
    }
}
