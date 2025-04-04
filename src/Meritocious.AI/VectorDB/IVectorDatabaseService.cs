
namespace Meritocious.AI.VectorDB
{
    public interface IVectorDatabaseService : IAsyncDisposable
    {
        // Index Management
        Task<bool> CreateIndexAsync(string indexName, int dimension);
        Task<bool> DeleteIndexAsync(string indexName);
        Task<Pinecone.Index> GetIndexAsync(string indexName);
        Task<List<Pinecone.Index>> ListIndexesAsync();
        Task ConfigureIndexAsync(string indexName, IndexConfigurationOptions options);
        Task<IndexStats> GetIndexStatsAsync(string indexName);

        // Vector Operations
        Task<bool> InsertVectorsAsync(string indexName, List<VectorEntry> vectors);
        Task<bool> UpdateVectorAsync(string indexName, VectorEntry vector);
        Task<bool> DeleteVectorsAsync(string indexName, List<string> ids);
        Task<List<SearchResult>> SearchAsync(
            string indexName,
            float[] queryVector,
            int topK = 10,
            SearchFilter filter = null);

        Task<bool> CreateCollectionAsync(string collectionName, int dimension);
    }
}