using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.SemanticKernel.Interfaces
{
    public interface ISemanticKernelService
    {
        Task<float[]> GetEmbeddingAsync(string input);
        Task<string> CompleteTextAsync(string prompt, Dictionary<string, object>? variables = null, Dictionary<string, PromptExecutionSettings>? settings = null);
    }
}