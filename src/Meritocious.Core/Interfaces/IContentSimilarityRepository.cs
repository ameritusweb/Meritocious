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
        Task<List<ContentSimilarity>> GetSimilarContentAsync(Guid contentId, decimal minSimilarity = 0.7m);
        Task<decimal> GetContentSimilarityAsync(Guid contentId1, Guid contentId2);
        Task<List<(Guid id1, Guid id2)>> GetContentPairsForUpdateAsync(int batchSize = 100);
        Task MarkForUpdateAsync(Guid contentId, int priority = 0);
        Task MarkOldSimilaritiesForUpdateAsync(TimeSpan age, int priority = 0);
        Task CreateMissingSimilaritiesAsync(List<Guid> contentIds);
    }
}
