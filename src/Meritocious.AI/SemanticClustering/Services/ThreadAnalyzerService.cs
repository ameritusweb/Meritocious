using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Meritocious.AI.Shared.Configuration;
using Meritocious.AI.SemanticClustering.Interfaces;
using Meritocious.AI.VectorDB;
using Meritocious.AI.SemanticKernel.Interfaces;

namespace Meritocious.AI.SemanticClustering.Services
{
    public class ThreadAnalyzerService : IThreadAnalyzer
    {
        private readonly ISemanticKernelService semanticKernelService;
        private readonly ILogger<ThreadAnalyzerService> logger;
        private readonly AIServiceConfiguration config;
        private readonly IVectorDatabaseService vectorDbService;

        public ThreadAnalyzerService(
            ISemanticKernelService semanticKernelService,
            IVectorDatabaseService vectorDbService,
            IOptions<AIServiceConfiguration> config,
            ILogger<ThreadAnalyzerService> logger)
        {
            this.semanticKernelService = semanticKernelService;
            this.vectorDbService = vectorDbService;
            this.config = config.Value;
            this.logger = logger;
        }

        public async Task<List<string>> ExtractKeyTopicsAsync(string content)
        {
            try
            {
                // Define the prompt for topic extraction
                var topicExtractionPrompt = @"Extract key topics from this text.
                Consider:
                1. Main themes and concepts
                2. Important terms and phrases
                3. Underlying ideas
                4. Domain-specific terminology

                Return topics as a comma-separated list of keywords or short phrases.
                Text: {{$text}}";

                var result = await semanticKernelService.CompleteTextAsync(
                    topicExtractionPrompt,
                    new Dictionary<string, object> { ["text"] = content });

                // Parse the comma-separated list
                var topics = result
                    .Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();

                // Generate embeddings for clustering
                var embeddings = new List<float[]>();
                foreach (var topic in topics)
                {
                    var embedding = await semanticKernelService.GetEmbeddingAsync(topic);
                    embeddings.Add(embedding);
                }

                // Cluster similar topics
                var clusters = ClusterTopics(topics, embeddings);

                // Rank clusters by importance
                var rankedTopics = await RankTopicClustersAsync(clusters, content);

                return rankedTopics;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error extracting key topics");
                return new List<string>();
            }
        }

        public async Task<double> CalculateSemanticSimilarityAsync(string content1, string content2)
        {
            try
            {
                // Generate embeddings
                var embedding1 = await semanticKernelService.GetEmbeddingAsync(content1);
                var embedding2 = await semanticKernelService.GetEmbeddingAsync(content2);

                // Calculate cosine similarity
                return CalculateCosineSimilarity(embedding1, embedding2);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating semantic similarity");
                return 0.0;
            }
        }

        public async Task<List<string>> SuggestRelatedThreadsAsync(string content, int maxResults = 5)
        {
            try
            {
                // Generate embedding for the content
                var contentEmbedding = await semanticKernelService.GetEmbeddingAsync(content);

                // Extract topics to use as a fallback
                var topics = await ExtractKeyTopicsAsync(content);

                // Search for related threads
                return await SearchRelatedThreadsAsync(contentEmbedding, topics, maxResults);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error suggesting related threads");
                return new List<string>();
            }
        }

        private async Task<List<string>> SearchRelatedThreadsAsync(
            float[] contentEmbedding,
            List<string> topics,
            int maxResults)
        {
            try
            {
                // Use the vector database service to search for similar content
                var relatedThreads = new List<string>();

                // Search for similar vectors in the 'threads' collection
                var searchResults = await vectorDbService.SearchAsync(
                    "meritocious_thread_vectors", // Collection name for threads
                    contentEmbedding,
                    maxResults);

                // Extract thread IDs from search results
                foreach (var result in searchResults)
                {
                    if (result.Metadata.TryGetValue("threadId", out var threadId))
                    {
                        relatedThreads.Add(threadId.ToString());
                    }
                }

                // If we found fewer related threads than requested, supplement with topic-based results
                if (relatedThreads.Count < maxResults && topics.Any())
                {
                    foreach (var topic in topics)
                    {
                        if (relatedThreads.Count >= maxResults)
                        {
                            break;
                        }

                        // Generate embedding for the topic
                        var topicEmbedding = await semanticKernelService.GetEmbeddingAsync(topic);

                        // Search for threads related to this topic
                        var topicResults = await vectorDbService.SearchAsync(
                            "meritocious_thread_vectors",
                            topicEmbedding,
                            maxResults - relatedThreads.Count);

                        // Add unique results to our list
                        foreach (var result in topicResults)
                        {
                            if (result.Metadata.TryGetValue("threadId", out var threadId) &&
                                !relatedThreads.Contains(threadId.ToString()))
                            {
                                relatedThreads.Add(threadId.ToString());

                                if (relatedThreads.Count >= maxResults)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                return relatedThreads;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching for related threads");

                // Fall back to basic topic-based results in case of error
                return topics.Take(maxResults).ToList();
            }
        }

        private double CalculateCosineSimilarity(float[] v1, float[] v2)
        {
            float dotProduct = 0;
            float norm1 = 0;
            float norm2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dotProduct += v1[i] * v2[i];
                norm1 += v1[i] * v1[i];
                norm2 += v2[i] * v2[i];
            }

            return dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        private async Task<List<string>> RankTopicClustersAsync(List<TopicCluster> clusters, string content)
        {
            var rankedTopics = new List<(string topic, double score)>();

            foreach (var cluster in clusters)
            {
                var clusterEmbedding = await semanticKernelService.GetEmbeddingAsync(cluster.MainTopic);
                var contentEmbedding = await semanticKernelService.GetEmbeddingAsync(content);

                double score = 0;

                // Cluster size score
                score += (cluster.RelatedTopics.Count + 1) * 0.2;

                // Semantic similarity score
                score += CalculateCosineSimilarity(clusterEmbedding, contentEmbedding) * 0.4;

                // Position score
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

        private List<TopicCluster> ClusterTopics(List<string> topics, List<float[]> embeddings)
        {
            var clusters = new List<TopicCluster>();
            var used = new HashSet<int>();

            for (int i = 0; i < topics.Count; i++)
            {
                if (used.Contains(i))
                {
                    continue;
                }

                var cluster = new TopicCluster { MainTopic = topics[i] };
                used.Add(i);

                // Find similar topics
                for (int j = i + 1; j < topics.Count; j++)
                {
                    if (used.Contains(j))
                    {
                        continue;
                    }

                    var similarity = CalculateCosineSimilarity(
                        embeddings[i],
                        embeddings[j]);

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

        private double CalculatePositionScore(TopicCluster cluster, string content)
        {
            var contentLower = content.ToLower();
            var firstPosition = contentLower.IndexOf(cluster.MainTopic.ToLower());

            if (firstPosition == -1)
            {
                return 0;
            }

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

        private class TopicCluster
        {
            public string MainTopic { get; set; }
            public List<string> RelatedTopics { get; set; } = new();
        }
    }
}