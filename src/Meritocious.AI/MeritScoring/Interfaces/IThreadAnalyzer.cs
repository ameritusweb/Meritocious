namespace Meritocious.AI.SemanticClustering.Interfaces
{
    public interface IThreadAnalyzer
    {
        Task<List<string>> ExtractKeyTopicsAsync(string content);
        Task<double> CalculateSemanticSimilarityAsync(string content1, string content2);
        Task<List<string>> SuggestRelatedThreadsAsync(string content, int maxResults = 5);
    }
}