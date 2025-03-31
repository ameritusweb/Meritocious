using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Shared.Configuration
{
    public class AIServiceConfiguration
    {
        public Dictionary<string, string> Embeddings { get; set; } = new();
        public Dictionary<string, string> Completion { get; set; } = new();
    }
}