using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Meritocious.AI.Shared.Configuration;
using System.Text.Json;
using Meritocious.AI.SemanticClustering.Interfaces;

namespace Meritocious.AI.SemanticClustering.Services
{
    public class ThreadAnalyzerService : IThreadAnalyzer
    {
        private readonly IKernel _semanticKernel;
        private readonly ILogger<ThreadAnalyzerService> _logger;
        private readonly AIServiceConfiguration _config;

        public ThreadAnalyzerService(
            IKernel semanticKernel,
            IOptions<AIServiceConfiguration> config,
            ILogger<ThreadAnalyzerService> logger)
        {
            _semanticKernel = semanticKernel;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<List<string>> ExtractKeyTopicsAsync(string content)
        {
            try
            {
                // 1. Extract potential topics using NLP
                var topics = await ExtractTopicCandidatesAsync(content);

                // 2. Generate embeddings for each topic
                var embeddings = await GenerateTopicEmbeddingsAsync(topics);

                // 3. Cluster similar topics
                var clusters = ClusterTopics(topics, embeddings);

                // 4. Rank clusters by importance
                var rankedTopics = await RankTopicClustersAsync(clusters, content);

                return rankedTopics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting key topics");
                return new List<string>();
            }
        }

        public async Task<double> CalculateSemanticSimilarityAsync(string content1, string content2)
        {
            try
            {
                // 1. Generate embeddings for both texts
                var embedding1 = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content1);
                var embedding2 = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content2);

                // 2. Calculate cosine similarity
                return CalculateCosineSimilarity(embedding1, embedding2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating semantic similarity");
                return 0.0;
            }
        }

        public async Task<List<string>> SuggestRelatedThreadsAsync(string content, int maxResults = 5)
        {
            try
            {
                // 1. Extract key topics from content
                var topics = await ExtractKeyTopicsAsync(content);

                // 2. Generate content embedding
                var contentEmbedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content);

                // 3. Find related threads using semantic search
                var relatedThreads = await SearchRelatedThreadsAsync(contentEmbedding, topics, maxResults);

                return relatedThreads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting related threads");
                return new List<string>();
            }
        }

        private async Task<List<string>> ExtractTopicCandidatesAsync(string content)
        {
            // Use semantic kernel for topic extraction
            var topicExtractionPrompt = @"Extract key topics from this text.
                                      Consider:
                                      1. Main themes and concepts
                                      2. Important terms and phrases
                                      3. Underlying ideas
                                      4. Domain-specific terminology

                                      Return topics as a JSON array of strings.
                                      Text: {{$text}}";

            var result = await _semanticKernel.InvokeSemanticFunctionAsync(topicExtractionPrompt, new { text = content });
            return JsonSerializer.Deserialize<List<string>>(result.ToString()) ?? new List<string>();
        }

        private async Task<List<ReadOnlyMemory<float>>> GenerateTopicEmbeddingsAsync(List<string> topics)
        {
            var embeddings = new List<ReadOnlyMemory<float>>();
            foreach (var topic in topics)
            {
                var embedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(topic);
                embeddings.Add(embedding);
            }
            return embeddings;
        }

        private List<TopicCluster> ClusterTopics(List<string> topics, List<ReadOnlyMemory<float>> embeddings)
        {
            var clusters = new List<TopicCluster>();
            var used = new HashSet<int>();

            for (int i = 0; i < topics.Count; i++)
            {
                if (used.Contains(i)) continue;

                var cluster = new TopicCluster { MainTopic = topics[i] };
                used.Add(i);

                // Find similar topics
                for (int j = i + 1; j < topics.Count; j++)
                {
                    if (used.Contains(j)) continue;

                    var similarity = CalculateCosineSimilarity(embeddings[i], embeddings[j]);
                    if (similarity > 0.7) // Similarity threshold
                    {
                        cluster.RelatedTopics.Add(topics[j]);
                        used.Add(j);
                    }
                }

                clusters.Add(cluster);
            }

            return clusters;
        }

        private async Task<List<string>> RankTopicClustersAsync(List<TopicCluster> clusters, string content)
        {
            // Score each cluster based on:
            // 1. Size of cluster (more related topics = more important)
            // 2. Semantic similarity to overall content
            // 3. Position of topics in the text
            // 4. Frequency of topic mentions

            var contentEmbedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content);
            var rankedTopics = new List<(string topic, double score)>();

            foreach (var cluster in clusters)
            {
                var clusterEmbedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(cluster.MainTopic);

                double score = 0;

                // Cluster size score
                score += (cluster.RelatedTopics.Count + 1) * 0.2;

                // Semantic similarity score
                score += CalculateCosineSimilarity(clusterEmbedding, contentEmbedding) * 0.4;

                // Position score (implement based on first occurrence in text)
                var positionScore = CalculatePositionScore(cluster, content);
                score += positionScore * 0.2;

                // Frequency score
                var frequencyScore = CalculateFrequencyScore(cluster, content);
                score += frequencyScore * 0.2;

                rankedTopics.Add((cluster.MainTopic, score));
            }

            return rankedTopics
                .OrderByDescending(t => t.score)
                .Select(t => t.topic)
                .ToList();
        }

        private double CalculatePositionScore(TopicCluster cluster, string content)
        {
            var contentLower = content.ToLower();
            var firstPosition = contentLower.IndexOf(cluster.MainTopic.ToLower());

            if (firstPosition == -1) return 0;

            // Earlier positions get higher scores
            return 1.0 - (firstPosition / (double)contentLower.Length);
        }

        private double CalculateFrequencyScore(TopicCluster cluster, string content)
        {
            var contentLower = content.ToLower();
            var mainTopicCount = System.Text.RegularExpressions.Regex
                .Matches(contentLower, cluster.MainTopic.ToLower())
                .Count;

            var relatedTopicsCounts = cluster.RelatedTopics.Sum(topic =>
                System.Text.RegularExpressions.Regex
                    .Matches(contentLower, topic.ToLower())
                    .Count);

            return Math.Min(1.0, (mainTopicCount + relatedTopicsCounts) * 0.1);
        }

        private async Task<List<string>> SearchRelatedThreadsAsync(
            ReadOnlyMemory<float> contentEmbedding,
            List<string> topics,
            int maxResults)
        {
            // In a real implementation, this would search a vector database
            // of thread embeddings and return the most similar threads

            // For now, return placeholder results
            return topics.Take(maxResults).ToList();
        }

        private double CalculateCosineSimilarity(ReadOnlyMemory<float> v1, ReadOnlyMemory<float> v2)
        {
            float dotProduct = 0;
            float norm1 = 0;
            float norm2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dotProduct += v1.Span[i] * v2.Span[i];
                norm1 += v1.Span[i] * v1.Span[i];
                norm2 += v2.Span[i] * v2.Span[i];
            }

            return dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        private class TopicCluster
        {
            public string MainTopic { get; set; }
            public List<string> RelatedTopics { get; set; } = new();
        }
    }
}