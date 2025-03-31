using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Clustering.Interfaces
{
    public interface ISemanticClusteringService
    {
        Task<List<string>> IdentifyTopicsAsync(List<string> contents);
        Task<double> CalculateSemanticSimilarityAsync(string content1, string content2);
        Task<List<string>> ExtractKeyTopicsAsync(string content);
        Task<List<string>> SuggestRelatedTopicsAsync(string topic, int maxResults = 5);
    }
}
