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
        Task IndexContentAsync(string contentId, ContentType contentType, string content);
        Task<List<SearchResult>> SearchSimilarContentAsync(string query, ContentType contentType, int maxResults = 10);
        Task<List<SearchResult>> FindSimilarContentAsync(string contentId, ContentType contentType, int maxResults = 10);
        Task UpdateContentAsync(string contentId, ContentType contentType, string newContent);
        Task DeleteContentAsync(string contentId, ContentType contentType);
        Task<float[]> GetContentEmbeddingAsync(string contentId, ContentType contentType);
        Task<decimal> CalculateContentSimilarityAsync(string contentId1, string contentId2, ContentType contentType);
        Task<List<(string contentId, decimal similarity)>> FindSemanticDuplicatesAsync(
            string content,
            ContentType contentType,
            decimal similarityThreshold = 0.95m);
    }
}
