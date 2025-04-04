namespace Meritocious.AI.VectorDB
{
    using Pinecone;
    using Pinecone.Core;

    public static class MetadataExtensions
    {
        public static Metadata ToMetadata(this Dictionary<string, object> dict)
        {
            var metadata = new Metadata();
            foreach (var kvp in dict)
            {
                metadata[kvp.Key] = ConvertToMetadataValue(kvp.Value);
            }

            return metadata;
        }

        private static MetadataValue ConvertToMetadataValue(object value)
        {
            return value switch
            {
                string s => s,
                double d => d,
                bool b => b,
                int i => (double)i,
                float f => (double)f,
                IEnumerable<string> ss => ss.ToList(),
                IEnumerable<double> ds => ds.ToList(),
                IEnumerable<bool> bs => bs.ToList(),
                IEnumerable<int> integers => integers.Select(i => (double)i).ToList(),
                IEnumerable<float> floats => floats.Select(f => (double)f).ToList(),
                Dictionary<string, object> nested => nested.ToMetadata(),
                _ => value.ToString()
            };
        }

        public static Dictionary<string, object> ToDictionary(this Metadata metadata)
        {
            var dict = new Dictionary<string, object>();
            foreach (var kvp in metadata)
            {
                if (kvp.Value != null)
                {
                    dict[kvp.Key] = ExtractValue<object>(kvp.Value);
                }
            }

            return dict;
        }

        private static T ExtractValue<T>(MetadataValue value)
        {
            return value.Match<T>(
                stringValue => (T)(object)stringValue,
                doubleValue => (T)(object)doubleValue,
                boolValue => (T)(object)boolValue,
                listValue => (T)(object)listValue.Select(v => v != null ? ExtractValue<object>(v) : null).ToList(),
                nestedMetadata => (T)(object)nestedMetadata.ToDictionary());
        }
    }

    public class VectorEntry
    {
        public string Id { get; set; }
        public float[] Vector { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();

        internal Pinecone.Vector ToPineconeVector()
        {
            return new Pinecone.Vector
            {
                Id = Id,
                Values = new ReadOnlyMemory<float>(Vector),
                Metadata = Metadata.ToMetadata()
            };
        }
    }

    public class SearchResult
    {
        public string Id { get; set; }
        public float? Score { get; set; }
        public float[]? Vector { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();

        internal static SearchResult FromPineconeMatch(ScoredVector match)
        {
            return new SearchResult
            {
                Id = match.Id,
                Score = match.Score,
                Vector = match.Values?.ToArray(),
                Metadata = match.Metadata?.ToDictionary() ?? new Dictionary<string, object>()
            };
        }
    }

    public class SearchFilter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }

        public static SearchFilter Create(string fieldName, object value)
        {
            return new SearchFilter
            {
                FieldName = fieldName,
                Value = value
            };
        }

        internal Metadata ToMetadata()
        {
            return new Dictionary<string, object>
            {
                { FieldName, Value }
            }.ToMetadata();
        }
    }
}