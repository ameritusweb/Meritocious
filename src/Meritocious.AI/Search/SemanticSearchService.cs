using Microsoft.Extensions.Logging;
using Meritocious.Core.Entities;
using System.Collections.Concurrent;
using Meritocious.Common.Enums;
using Meritocious.AI.VectorDB;
using Meritocious.AI.MeritScoring.Interfaces;
using Meritocious.AI.SemanticKernel.Interfaces;
using Pinecone;

namespace Meritocious.AI.Search
{
    public class SemanticSearchService : ISemanticSearchService
    {
        private readonly IVectorDatabaseService vectorDb;
        private readonly ISemanticKernelService semanticKernel;
        private readonly ILogger<SemanticSearchService> logger;
        private readonly ConcurrentDictionary<ContentType, string> collectionNames;

        private const int VECTOR_DIMENSION = 1536; // OpenAI ada-002 embedding dimension

        public SemanticSearchService(
            IVectorDatabaseService vectorDb,
            ISemanticKernelService semanticKernel,
            ILogger<SemanticSearchService> logger)
        {
            this.vectorDb = vectorDb;
            this.semanticKernel = semanticKernel;
            this.logger = logger;
            collectionNames = new ConcurrentDictionary<ContentType, string>();
        }

        public async Task InitializeCollectionsAsync()
        {
            try
            {
                foreach (ContentType contentType in Enum.GetValues(typeof(ContentType)))
                {
                    var collectionName = GetCollectionName(contentType);
                    await vectorDb.CreateCollectionAsync(collectionName, VECTOR_DIMENSION);
                    collectionNames[contentType] = collectionName;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initializing vector collections");
                throw;
            }
        }

        public async Task IndexContentAsync(Guid contentId, ContentType contentType, string content)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);
                var embedding = await semanticKernel.GetEmbeddingAsync(content);

                var vector = new VectorEntry
                {
                    Id = contentId.ToString(),
                    Vector = embedding.ToArray(),
                    Metadata = new Dictionary<string, object>
                    {
                        ["contentType"] = contentType.ToString(),
                        ["indexedAt"] = DateTime.UtcNow.ToString("O")
                    }
                };

                await vectorDb.InsertVectorsAsync(collectionName, new[] { vector }.ToList());
                logger.LogInformation("Indexed content {ContentId} of type {ContentType}",
                    contentId, contentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error indexing content {ContentId} of type {ContentType}",
                    contentId, contentType);
                throw;
            }
        }

        public async Task<List<SearchResult>> SearchSimilarContentAsync(
            string query,
            ContentType contentType,
            int maxResults = 10)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);
                var queryEmbedding = await semanticKernel.GetEmbeddingAsync(query);

                return await vectorDb.SearchAsync(
                    collectionName,
                    queryEmbedding.ToArray(),
                    maxResults);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching similar content for query in {ContentType}",
                    contentType);
                throw;
            }
        }

        public async Task<List<SearchResult>> FindSimilarContentAsync(
            Guid contentId,
            ContentType contentType,
            int maxResults = 10)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);

                // Get the embedding for the source content
                var sourceVector = await GetContentEmbeddingAsync(contentId, contentType);
                if (sourceVector == null)
                {
                    throw new InvalidOperationException($"Content {contentId} not found in vector database");
                }

                return await vectorDb.SearchAsync(
                    collectionName,
                    sourceVector,
                    maxResults + 1); // Add 1 to account for the source content itself
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error finding similar content for {ContentId} of type {ContentType}",
                    contentId, contentType);
                throw;
            }
        }

        public async Task UpdateContentAsync(Guid contentId, ContentType contentType, string newContent)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);
                var embedding = await semanticKernel.GetEmbeddingAsync(newContent);

                var vector = new VectorEntry
                {
                    Id = contentId.ToString(),
                    Vector = embedding.ToArray(),
                    Metadata = new Dictionary<string, object>
                    {
                        ["contentType"] = contentType.ToString(),
                        ["indexedAt"] = DateTime.UtcNow.ToString("O"),
                        ["updatedAt"] = DateTime.UtcNow.ToString("O")
                    }
                };

                await vectorDb.UpdateVectorAsync(collectionName, vector);
                logger.LogInformation("Updated content {ContentId} of type {ContentType} in vector database",
                    contentId, contentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating content {ContentId} of type {ContentType} in vector database",
                    contentId, contentType);
                throw;
            }
        }

        public async Task DeleteContentAsync(Guid contentId, ContentType contentType)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);
                await vectorDb.DeleteVectorsAsync(collectionName, new[] { contentId.ToString() }.ToList());

                logger.LogInformation("Deleted content {ContentId} of type {ContentType} from vector database",
                    contentId, contentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting content {ContentId} of type {ContentType} from vector database",
                    contentId, contentType);
                throw;
            }
        }

        public async Task<float[]> GetContentEmbeddingAsync(Guid contentId, ContentType contentType)
        {
            try
            {
                var collectionName = GetCollectionName(contentType);
                var results = await vectorDb.SearchAsync(
                    collectionName,
                    new float[VECTOR_DIMENSION], // Dummy vector for exact ID match
                    1,
                    new SearchFilter
                    {
                        FieldName = "id",
                        Value = new MetadataValue(contentId.ToString())
                    });

                var result = results.FirstOrDefault();
                return result?.Vector;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting embedding for content {ContentId} of type {ContentType}",
                    contentId, contentType);
                throw;
            }
        }

        public async Task<decimal> CalculateContentSimilarityAsync(
            Guid contentId1,
            Guid contentId2,
            ContentType contentType)
        {
            try
            {
                var embedding1 = await GetContentEmbeddingAsync(contentId1, contentType);
                var embedding2 = await GetContentEmbeddingAsync(contentId2, contentType);

                if (embedding1 == null || embedding2 == null)
                {
                    throw new InvalidOperationException("One or both content items not found in vector database");
                }

                return (decimal)CalculateCosineSimilarity(embedding1, embedding2);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating similarity between content {ContentId1} and {ContentId2}",
                    contentId1, contentId2);
                throw;
            }
        }

        public async Task<List<(Guid contentId, decimal similarity)>> FindSemanticDuplicatesAsync(
            string content,
            ContentType contentType,
            decimal similarityThreshold = 0.95m)
        {
            try
            {
                var embedding = await semanticKernel.GetEmbeddingAsync(content);
                var collectionName = GetCollectionName(contentType);

                var results = await vectorDb.SearchAsync(
                    collectionName,
                    embedding.ToArray(),
                    100); // Get more results to find all potential duplicates

                return results
                    .Where(r => r.Score >= (float)similarityThreshold)
                    .Select(r => (Guid.Parse(r.Id), (decimal)r.Score))
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error finding semantic duplicates in {ContentType}", contentType);
                throw;
            }
        }

        private string GetCollectionName(ContentType contentType) =>
            $"meritocious_{contentType.ToString().ToLower()}_vectors";

        private float CalculateCosineSimilarity(float[] v1, float[] v2)
        {
            float dotProduct = 0;
            float norm1 = 0;
            float norm2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dotProduct += v1[i] * v2[i];
                norm1 += v1[i] * v1[i];
                norm2 += v2[i] * v2[i];
            }

            return dotProduct / (float)Math.Sqrt(norm1 * norm2);
        }
    }
}