using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Meritocious.AI.Shared.Configuration;
using Meritocious.Core.Entities;
using System.Text.Json;
using Meritocious.AI.MeritScoring.Interfaces;
using Meritocious.AI.SemanticClustering.Interfaces;
using Meritocious.AI.SemanticClustering.Services;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.AI.Recommendations.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IKernel _semanticKernel;
        private readonly ILogger<RecommendationService> _logger;
        private readonly AIServiceConfiguration _config;
        private readonly IThreadAnalyzer _threadAnalyzer;

        public RecommendationService(
            IKernel semanticKernel,
            IThreadAnalyzer threadAnalyzer,
            IOptions<AIServiceConfiguration> config,
            ILogger<RecommendationService> logger)
        {
            _semanticKernel = semanticKernel;
            _threadAnalyzer = threadAnalyzer;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<List<ContentRecommendation>> GetRecommendationsAsync(
            Guid userId,
            List<UserInteractionHistory> userHistory,
            int count = 10,
            List<string> excludedContentIds = null)
        {
            try
            {
                // 1. Build user profile
                var userProfile = await BuildUserProfileAsync(userId, userHistory);

                // 2. Get recommendations from different strategies
                var tasks = new[]
                {
                    GetCollaborativeFilteringRecommendationsAsync(userProfile, count),
                    GetContentBasedRecommendationsAsync(userProfile, count),
                    GetTrendingRecommendationsAsync(count),
                    GetSemanticRecommendationsAsync(userProfile, count)
                };

                var results = await Task.WhenAll(tasks);

                // 3. Merge and rank recommendations
                var mergedRecommendations = MergeRecommendations(results.SelectMany(r => r).ToList());

                // 4. Filter out excluded content
                if (excludedContentIds != null && excludedContentIds.Any())
                {
                    mergedRecommendations = mergedRecommendations
                        .Where(r => !excludedContentIds.Contains(r.ContentId.ToString()))
                        .ToList();
                }

                // 5. Personalize explanations
                await PersonalizeRecommendationExplanationsAsync(mergedRecommendations, userProfile);

                return mergedRecommendations
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating recommendations for user {UserId}", userId);
                return new List<ContentRecommendation>();
            }
        }

        private async Task<UserProfile> BuildUserProfileAsync(
            Guid userId,
            List<UserInteractionHistory> userHistory)
        {
            var profile = new UserProfile
            {
                UserId = userId,
                InteractionHistory = userHistory,
                TopicPreferences = new Dictionary<string, decimal>(),
                InteractionPatterns = new Dictionary<string, decimal>()
            };

            // Analyze interaction history
            foreach (var interaction in userHistory.OrderByDescending(h => h.Timestamp))
            {
                // Extract topics from content
                var topics = await _threadAnalyzer.ExtractKeyTopicsAsync(interaction.ContentId.ToString());

                foreach (var topic in topics)
                {
                    if (!profile.TopicPreferences.ContainsKey(topic))
                    {
                        profile.TopicPreferences[topic] = 0;
                    }

                    // Weight recent interactions more heavily
                    var recencyWeight = CalculateRecencyWeight(interaction.Timestamp);
                    var interactionWeight = CalculateInteractionWeight(interaction.InteractionType);

                    profile.TopicPreferences[topic] += recencyWeight * interactionWeight * interaction.EngagementLevel;
                }

                // Track interaction patterns
                var pattern = interaction.InteractionType;
                if (!profile.InteractionPatterns.ContainsKey(pattern))
                {
                    profile.InteractionPatterns[pattern] = 0;
                }
                profile.InteractionPatterns[pattern] += recencyWeight;
            }

            // Normalize weights
            NormalizeWeights(profile.TopicPreferences);
            NormalizeWeights(profile.InteractionPatterns);

            return profile;
        }

        private async Task<List<ContentRecommendation>> GetCollaborativeFilteringRecommendationsAsync(
            UserProfile userProfile,
            int count)
        {
            // Find similar users based on interaction patterns
            var similarUsers = await FindSimilarUsersAsync(userProfile);
            var recommendations = new List<ContentRecommendation>();

            foreach (var user in similarUsers)
            {
                // Get content that similar users engaged with positively
                var userContent = await GetUserPositiveInteractionsAsync(user.UserId);

                foreach (var content in userContent)
                {
                    // Check if user hasn't interacted with this content
                    if (!userProfile.InteractionHistory.Any(h => h.ContentId == content.ContentId))
                    {
                        recommendations.Add(new ContentRecommendation
                        {
                            ContentId = content.ContentId,
                            RecommendationType = "collaborative",
                            RelevanceScore = user.Similarity * content.EngagementScore,
                            Reason = $"Popular among users with similar interests"
                        });
                    }
                }
            }

            return recommendations
                .OrderByDescending(r => r.RelevanceScore)
                .Take(count)
                .ToList();
        }

        private async Task<List<ContentRecommendation>> GetContentBasedRecommendationsAsync(
            UserProfile userProfile,
            int count)
        {
            var recommendations = new List<ContentRecommendation>();

            // Get recent content that matches user's topic preferences
            foreach (var topic in userProfile.TopicPreferences.OrderByDescending(t => t.Value))
            {
                var topicContent = await GetRecentContentByTopicAsync(topic.Key);

                foreach (var content in topicContent)
                {
                    // Skip if user has already interacted
                    if (userProfile.InteractionHistory.Any(h => h.ContentId == content.ContentId))
                        continue;

                    // Calculate content relevance based on topic preference
                    var relevanceScore = CalculateContentRelevance(content, userProfile);

                    recommendations.Add(new ContentRecommendation
                    {
                        ContentId = content.ContentId,
                        RecommendationType = "content_based",
                        RelevanceScore = relevanceScore,
                        Reason = $"Matches your interest in {topic.Key}"
                    });
                }
            }

            return recommendations
                .OrderByDescending(r => r.RelevanceScore)
                .Take(count)
                .ToList();
        }

        private async Task<List<ContentRecommendation>> GetSemanticRecommendationsAsync(
            UserProfile userProfile,
            int count)
        {
            var recommendations = new List<ContentRecommendation>();

            // Get embeddings for user's positive interactions
            var interactionEmbeddings = new List<(Guid contentId, ReadOnlyMemory<float> embedding)>();

            foreach (var interaction in userProfile.InteractionHistory
                .Where(h => h.EngagementLevel > 0.7m)
                .OrderByDescending(h => h.Timestamp)
                .Take(10))
            {
                var content = await GetContentByIdAsync(interaction.ContentId);
                var embedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content);
                interactionEmbeddings.Add((interaction.ContentId, embedding));
            }

            // Find semantically similar content
            var recentContent = await GetRecentContentAsync(100);
            foreach (var content in recentContent)
            {
                // Skip if user has already interacted
                if (userProfile.InteractionHistory.Any(h => h.ContentId == content.ContentId))
                    continue;

                var contentEmbedding = await _semanticKernel.Memory.Embeddings.GenerateEmbeddingAsync(content.Value);

                // Calculate average similarity to user's positive interactions
                var avgSimilarity = interactionEmbeddings
                    .Select(e => CalculateCosineSimilarity(contentEmbedding, e.embedding))
                    .Average();

                if (avgSimilarity > 0.7) // Similarity threshold
                {
                    recommendations.Add(new ContentRecommendation
                    {
                        ContentId = content.ContentId,
                        RecommendationType = "semantic",
                        RelevanceScore = (decimal)avgSimilarity,
                        Reason = "Similar to content you've enjoyed"
                    });
                }
            }

            return recommendations
                .OrderByDescending(r => r.RelevanceScore)
                .Take(count)
                .ToList();
        }

        private async Task<List<ContentRecommendation>> GetTrendingRecommendationsAsync(int count)
        {
            // Get trending content based on recent engagement and merit scores
            var trendingContent = await GetTrendingContentAsync();

            return trendingContent.Select(c => new ContentRecommendation
            {
                ContentId = c.ContentId,
                RecommendationType = "trending",
                RelevanceScore = c.TrendingScore,
                Reason = "Trending in the community"
            })
            .Take(count)
            .ToList();
        }

        private List<ContentRecommendation> MergeRecommendations(List<ContentRecommendation> recommendations)
        {
            // Group by content ID to handle duplicates from different strategies
            var grouped = recommendations.GroupBy(r => r.ContentId);
            var merged = new List<ContentRecommendation>();

            foreach (var group in grouped)
            {
                var bestRecommendation = group
                    .OrderByDescending(r => r.RelevanceScore)
                    .First();

                // Boost score if recommended by multiple strategies
                if (group.Count() > 1)
                {
                    bestRecommendation.RelevanceScore = Math.Min(1.0m,
                        bestRecommendation.RelevanceScore * (1 + 0.1m * (group.Count() - 1)));
                }

                merged.Add(bestRecommendation);
            }

            return merged.OrderByDescending(r => r.RelevanceScore).ToList();
        }

        private async Task PersonalizeRecommendationExplanationsAsync(
            List<ContentRecommendation> recommendations,
            UserProfile userProfile)
        {
            foreach (var recommendation in recommendations)
            {
                var content = await GetContentByIdAsync(recommendation.ContentId);
                var topics = await _threadAnalyzer.ExtractKeyTopicsAsync(content);

                // Find matching interests
                var matchingTopics = topics
                    .Where(t => userProfile.TopicPreferences.ContainsKey(t))
                    .OrderByDescending(t => userProfile.TopicPreferences[t])
                    .Take(2)
                    .ToList();

                if (matchingTopics.Any())
                {
                    recommendation.Reason += $" and aligns with your interest in {string.Join(" and ", matchingTopics)}";
                }
            }
        }

        #region Helper Methods

        private decimal CalculateRecencyWeight(DateTime timestamp)
        {
            var age = DateTime.UtcNow - timestamp;
            return (decimal)Math.Exp(-age.TotalDays / 30); // Exponential decay over 30 days
        }

        private decimal CalculateInteractionWeight(string interactionType)
        {
            return interactionType switch
            {
                "author" => 1.0m,
                "comment" => 0.8m,
                "like" => 0.6m,
                "view" => 0.3m,
                _ => 0.1m
            };
        }

        private void NormalizeWeights(Dictionary<string, decimal> weights)
        {
            var sum = weights.Values.Sum();
            if (sum > 0)
            {
                foreach (var key in weights.Keys.ToList())
                {
                    weights[key] /= sum;
                }
            }
        }

        private float CalculateCosineSimilarity(ReadOnlyMemory<float> v1, ReadOnlyMemory<float> v2)
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

            return dotProduct / (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        #endregion

        #region Data Access Methods
        // These methods would be implemented to interact with your actual data store

        private async Task<List<SimilarUser>> FindSimilarUsersAsync(UserProfile userProfile)
        {
            // TODO: Implement actual user similarity search
            return new List<SimilarUser>();
        }

        private async Task<List<ContentInteraction>> GetUserPositiveInteractionsAsync(Guid userId)
        {
            // TODO: Implement actual user interaction retrieval
            return new List<ContentInteraction>();
        }

        private async Task<List<Content>> GetRecentContentByTopicAsync(string topic)
        {
            // TODO: Implement actual content retrieval
            return new List<Content>();
        }

        private async Task<string> GetContentByIdAsync(Guid contentId)
        {
            // TODO: Implement actual content retrieval
            return string.Empty;
        }

        private async Task<List<Content>> GetRecentContentAsync(int count)
        {
            // TODO: Implement actual content retrieval
            return new List<Content>();
        }

        private async Task<List<TrendingContent>> GetTrendingContentAsync()
        {
            // TODO: Implement actual trending content retrieval
            return new List<TrendingContent>();
        }

        #endregion

        #region Models

        private class UserProfile
        {
            public Guid UserId { get; set; }
            public List<UserInteractionHistory> InteractionHistory { get; set; }
            public Dictionary<string, decimal> TopicPreferences { get; set; }
            public Dictionary<string, decimal> InteractionPatterns { get; set; }
        }

        private class SimilarUser
        {
            public Guid UserId { get; set; }
            public decimal Similarity { get; set; }
        }

        private class ContentInteraction
        {
            public Guid ContentId { get; set; }
            public decimal EngagementScore { get; set; }
        }

        private class Content
        {
            public Guid ContentId { get; set; }
            public string Value { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private class TrendingContent
        {
            public Guid ContentId { get; set; }
            public decimal TrendingScore { get; set; }
        }

        #endregion
    }
}