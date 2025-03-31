using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.MeritScoring.Interfaces
{
    public interface ISemanticClusteringService
    {
        Task<List<string>> IdentifyTopicsAsync(List<string> contents);
        Task<List<string>> GetRelatedTagsAsync(string topic);
    }
}
