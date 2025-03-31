using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Shared.Configuration
{
    public class AIServiceConfiguration
    {
        public string ModelEndpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int MaxRetries { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
        public decimal MinimumConfidenceThreshold { get; set; } = 0.7m;
    }
}