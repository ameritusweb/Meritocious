using Meritocious.AI.VectorDB;
using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.MeritScoring.Interfaces
{
    public interface ISemanticSearchService
    {
        Task InitializeCollectionsAsync();
        Task IndexContentAsync(Guid contentId, ContentType contentType, string content);
        Task<List<SearchResult>> SearchSimilarContentAsync(string query, ContentType contentType, int maxResults = 10);
        Task<List<SearchResult>> FindSimilarContentAsync(Guid contentId, ContentType contentType, int maxResults = 10);
        Task UpdateContentAsync(Guid contentId, ContentType contentType, string newContent);
        Task DeleteContentAsync(Guid contentId, ContentType contentType);
        Task<float[]> GetContentEmbeddingAsync(Guid contentId, ContentType contentType);
        Task<decimal> CalculateContentSimilarityAsync(Guid contentId1, Guid contentId2, ContentType contentType);
        Task<List<(Guid contentId, decimal similarity)>> FindSemanticDuplicatesAsync(
            string content,
            ContentType contentType,
            decimal similarityThreshold = 0.95m);
    }
}
