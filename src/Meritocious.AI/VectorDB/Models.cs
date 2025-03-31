using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.VectorDB
{
    public class VectorDBSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }

    public class IndexSettings
    {
        public string PodType { get; set; } = "p1.x1";
        public int Pods { get; set; } = 1;
        public string Metric { get; set; } = "cosine";
        public bool Replicas { get; set; } = false;
        public Dictionary<string, string> MetadataConfig { get; set; } = new();
    }
}
