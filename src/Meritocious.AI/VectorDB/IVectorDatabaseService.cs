using System.Numerics;

namespace Meritocious.AI.VectorDB
{
    public interface IVectorDatabaseService
    {
        Task<bool> CreateCollectionAsync(string collectionName, int dimension);
        Task<bool> DeleteCollectionAsync(string collectionName);
        Task<bool> InsertVectorsAsync(string collectionName, List<VectorEntry> vectors);
        Task<List<SearchResult>> SearchAsync(string collectionName, float[] queryVector, int topK = 10);
        Task<bool> DeleteVectorsAsync(string collectionName, List<string> ids);
        Task<bool> UpdateVectorAsync(string collectionName, VectorEntry vector);
    }

    public class VectorEntry
    {
        public string Id { get; set; }
        public float[] Vector { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class SearchResult
    {
        public string Id { get; set; }
        public float Score { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}