using Microsoft.Extensions.DependencyInjection;
using Meritocious.AI.Search;
using Meritocious.AI.VectorDB;
using Meritocious.AI.MeritScoring.Interfaces;
using Meritocious.AI.SemanticClustering.Services;
using Meritocious.Common.Enums;
using Meritocious.Core.Interfaces;

namespace Meritocious.AI.Recommendations
{
    public static class RecommendationServiceExtensions
    {
        public static IServiceCollection AddVectorRecommendations(
            this IServiceCollection services,
            Action<VectorDBSettings> configureVectorDb)
        {
            // Configure vector database
            services.Configure(configureVectorDb);
            services.AddSingleton<IVectorDatabaseService, PineconeVectorDatabaseService>();

            // Add semantic search services
            services.AddScoped<ISemanticSearchService, SemanticSearchService>();

            // Add vector index maintenance
            services.AddHostedService<VectorIndexMaintenanceService>();

            return services;
        }
    }

    public static class RecommendationServiceSemanticExtensions
    {
        public static async Task<List<ContentRecommendation>> GetSemanticRecommendationsAsync(
            this IRecommendationService recommendationService,
            ISemanticSearchService semanticSearch,
            UserProfile userProfile,
            int count)
        {
            // Get embeddings for user's recent positive interactions
            var recommendations = new List<ContentRecommendation>();
            var processedContentIds = new HashSet<string>();

            foreach (var interaction in userProfile.InteractionHistory
                .Where(h => h.EngagementLevel > 0.7m)
                .OrderByDescending(h => h.Timestamp)
                .Take(5))
            {
                var similarContent = await semanticSearch.FindSimilarContentAsync(
                    interaction.ContentId,
                    interaction.ContentType,
                    maxResults: count * 2);

                foreach (var content in similarContent)
                {
                    if (processedContentIds.Contains(content.Id))
                    {
                        continue;
                    }

                    // Skip content the user has already interacted with
                    if (userProfile.InteractionHistory.Any(h => h.ContentId.ToString() == content.Id))
                    {
                        continue;
                    }

                    processedContentIds.Add(content.Id);
                    recommendations.Add(new ContentRecommendation
                    {
                        ContentId = content.Id,
                        RecommendationType = "semantic",
                        RelevanceScore = (decimal)content.Score,
                        Reason = "Similar to content you've engaged with positively"
                    });
                }
            }

            // Process topic-based semantic recommendations
            foreach (var topic in userProfile.TopicPreferences
                .OrderByDescending(t => t.Value)
                .Take(3))
            {
                var topicResults = await semanticSearch.SearchSimilarContentAsync(
                    topic.Key,  // Use topic as search query
                    ContentType.Post,  // Assuming we're recommending posts
                    count);

                foreach (var content in topicResults)
                {
                    if (processedContentIds.Contains(content.Id))
                    {
                        continue;
                    }

                    if (userProfile.InteractionHistory.Any(h => h.ContentId.ToString() == content.Id))
                    {
                        continue;
                    }

                    processedContentIds.Add(content.Id);
                    recommendations.Add(new ContentRecommendation
                    {
                        ContentId = content.Id,
                        RecommendationType = "topic_semantic",
                        RelevanceScore = (decimal)content.Score * topic.Value,
                        Reason = $"Matches your interest in {topic.Key}"
                    });
                }
            }

            return recommendations
                .OrderByDescending(r => r.RelevanceScore)
                .Take(count)
                .ToList();
        }

        public static async Task<List<ContentRecommendation>> GetDiverseRecommendationsAsync(
            this IRecommendationService recommendationService,
            ISemanticSearchService semanticSearch,
            UserProfile userProfile,
            int count)
        {
            var recommendations = new List<ContentRecommendation>();
            var processedContentIds = new HashSet<string>();
            var minSemanticDistance = 0.3f; // Minimum semantic distance for diversity

            // Get initial recommendations
            var initialRecs = await recommendationService.GetSemanticRecommendationsAsync(
                semanticSearch,
                userProfile,
                count * 2);

            foreach (var rec in initialRecs)
            {
                if (recommendations.Count >= count)
                {
                    break;
                }

                var isUnique = true;
                var recEmbedding = await semanticSearch.GetContentEmbeddingAsync(
                    rec.ContentId,
                    ContentType.Post);

                // Check semantic similarity with already selected recommendations
                foreach (var selected in recommendations)
                {
                    var selectedEmbedding = await semanticSearch.GetContentEmbeddingAsync(
                        selected.ContentId,
                        ContentType.Post);

                    var similarity = CalculateCosineSimilarity(recEmbedding, selectedEmbedding);
                    if (similarity > (1 - minSemanticDistance))
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                {
                    recommendations.Add(rec);
                }
            }

            return recommendations;
        }

        public static async Task<List<ContentRecommendation>> GetSerendipitousRecommendationsAsync(
            this IRecommendationService recommendationService,
            ISemanticSearchService semanticSearch,
            UserProfile userProfile,
            int count)
        {
            var recommendations = new List<ContentRecommendation>();
            var processedContentIds = new HashSet<string>();

            // Get topics that are semantically related but not identical to user's interests
            foreach (var topic in userProfile.TopicPreferences
                .OrderByDescending(t => t.Value)
                .Take(3))
            {
                var relatedTopics = await semanticSearch.SearchSimilarContentAsync(
                    topic.Key,
                    ContentType.Post,
                    5);

                foreach (var relatedTopic in relatedTopics.Where(t => t.Score >= 0.6 && t.Score <= 0.8))
                {
                    var topicResults = await semanticSearch.SearchSimilarContentAsync(
                        relatedTopic.Metadata["topic"].ToString(),
                        ContentType.Post,
                        count);

                    foreach (var content in topicResults)
                    {
                        if (processedContentIds.Contains(content.Id))
                        {
                            continue;
                        }

                        if (userProfile.InteractionHistory.Any(h => h.ContentId.ToString() == content.Id))
                        {
                            continue;
                        }

                        processedContentIds.Add(content.Id);
                        recommendations.Add(new ContentRecommendation
                        {
                            ContentId = content.Id,
                            RecommendationType = "serendipitous",
                            RelevanceScore = (decimal)content.Score * 0.8m,
                            Reason = $"Something different based on your interest in {topic.Key}"
                        });
                    }
                }
            }

            return recommendations
                .OrderByDescending(r => r.RelevanceScore)
                .Take(count)
                .ToList();
        }

        private static float CalculateCosineSimilarity(float[] v1, float[] v2)
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

            return dotProduct / (float)Math.Sqrt(norm1 * norm2);
        }
    }
}