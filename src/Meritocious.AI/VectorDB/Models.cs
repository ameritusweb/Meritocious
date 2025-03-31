using Pinecone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.VectorDB
{
    public class VectorDBSettings
    {
        public string ApiKey { get; set; }
        public string Environment { get; set; }
        public string ProjectId { get; set; }
        public IndexSettings DefaultIndex { get; set; } = new();
    }

    public class IndexSettings
    {
        public string PodType { get; set; } = "p1.x1";
        public int Pods { get; set; } = 1;
        public int? Replicas { get; set; }
        public int? Shards { get; set; }
        public string? SourceCollection { get; set; }
        public MetadataIndexConfig MetadataConfig { get; set; } = new();
    }

    public class MetadataIndexConfig
    {
        public IEnumerable<string> Indexed { get; set; } = new[]
        {
            "contentType",
            "indexedAt",
            "updatedAt"
        };
    }

    public class IndexConfigurationOptions
    {
        public int? Replicas { get; set; }
        public string? PodType { get; set; }
        public DeletionProtection? DeletionProtection { get; set; }
        public Dictionary<string, string>? Tags { get; set; }
        public EmbeddingConfiguration? EmbeddingConfig { get; set; }
    }

    public class IndexStats
    {
        public int Dimension { get; set; }
        public long TotalVectorCount { get; set; }
        public string Namespace { get; set; }
    }

    public class EmbeddingConfiguration
    {
        public string? Model { get; set; }
        public Dictionary<string, object?>? FieldMap { get; set; }
        public Dictionary<string, object?>? ReadParameters { get; set; }
        public Dictionary<string, object?>? WriteParameters { get; set; }
    }
}
