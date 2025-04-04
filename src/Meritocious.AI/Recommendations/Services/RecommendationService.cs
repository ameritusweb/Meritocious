using Meritocious.AI.SemanticClustering.Interfaces;
using Meritocious.AI.Shared.Configuration;
using Meritocious.Core.Features.Recommendations.Models;
using Meritocious.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Meritocious.AI.SemanticKernel.Interfaces;

namespace Meritocious.AI.Recommendations.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ISemanticKernelService semanticKernel;
        private readonly ILogger<RecommendationService> logger;
        private readonly AIServiceConfiguration config;
        private readonly IThreadAnalyzer threadAnalyzer;
        private readonly IUserTopicPreferenceRepository preferenceRepo;
        private readonly IUserInteractionRepository interactionRepo;
        private readonly IContentTopicRepository topicRepo;
        private readonly IPostRepository postRepo;
        private readonly ITrendingContentRepository trendingRepo;

        public RecommendationService(
            ISemanticKernelService semanticKernel,
            IThreadAnalyzer threadAnalyzer,
            IOptions<AIServiceConfiguration> config,
            IUserTopicPreferenceRepository preferenceRepo,
            IUserInteractionRepository interactionRepo,
            IContentTopicRepository topicRepo,
            IPostRepository postRepo,
            ITrendingContentRepository trendingRepo,
            ILogger<RecommendationService> logger)
        {
            this.semanticKernel = semanticKernel;
            this.threadAnalyzer = threadAnalyzer;
            this.config = config.Value;
            this.logger = logger;
            this.preferenceRepo = preferenceRepo;
            this.interactionRepo = interactionRepo;
            this.topicRepo = topicRepo;
            this.postRepo = postRepo;
            this.trendingRepo = trendingRepo;
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
                logger.LogError(ex, "Error generating recommendations for user {UserId}", userId);
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
                var topics = await threadAnalyzer.ExtractKeyTopicsAsync(interaction.ContentId.ToString());

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

                // TODO: Add recency weight.
                // profile.InteractionPatterns[pattern] += recencyWeight;
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
                    {
                        continue;
                    }

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

        private decimal CalculateContentRelevance(Content content, UserProfile userProfile)
        {
            // TODO: Calculate content relevance.
            throw new NotImplementedException();
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
                var embedding = await semanticKernel.GetEmbeddingAsync(content);
                interactionEmbeddings.Add((interaction.ContentId, new ReadOnlyMemory<float>(embedding)));
            }

            // Find semantically similar content
            var recentContent = await GetRecentContentAsync(100);
            foreach (var content in recentContent)
            {
                // Skip if user has already interacted
                if (userProfile.InteractionHistory.Any(h => h.ContentId == content.ContentId))
                {
                    continue;
                }

                var contentEmbedding = await semanticKernel.GetEmbeddingAsync(content.Value);

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
                var topics = await threadAnalyzer.ExtractKeyTopicsAsync(content);

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

        private async Task<List<SimilarUser>> FindSimilarUsersAsync(UserProfile userProfile)
        {
            // Find users with similar topic preferences
            var similarUsers = new List<SimilarUser>();
            foreach (var topic in userProfile.TopicPreferences
                .OrderByDescending(t => t.Value)
                .Take(5))
            {
                var users = await preferenceRepo.GetUsersInterestedInTopicAsync(
                    topic.Key,
                    limit: 10,
                    minWeight: 0.5m);

                foreach (var user in users.Where(u => u.Id != userProfile.UserId.ToString()))
                {
                    var userPrefs = await preferenceRepo.GetUserTopicWeightsAsync(Guid.Parse(user.Id));
                    var similarity = CalculatePreferenceSimilarity(
                        userProfile.TopicPreferences,
                        userPrefs);

                    similarUsers.Add(new SimilarUser
                    {
                        UserId = Guid.Parse(user.Id),
                        Similarity = similarity
                    });
                }
            }

            return similarUsers
                .OrderByDescending(u => u.Similarity)
                .Take(10)
                .ToList();
        }

        private async Task<List<ContentInteraction>> GetUserPositiveInteractionsAsync(Guid userId)
        {
            var interactions = await interactionRepo.GetUserInteractionsAsync(
                userId,
                since: DateTime.UtcNow.AddDays(-30));

            return interactions
                .Where(i => i.EngagementScore >= 0.7m)
                .Select(i => new ContentInteraction
                {
                    ContentId = i.ContentId,
                    EngagementScore = i.EngagementScore
                })
                .ToList();
        }

        private async Task<List<Content>> GetRecentContentByTopicAsync(string topic)
        {
            var content = await topicRepo.GetTopicContentAsync(
                topic,
                limit: 50,
                minRelevance: 0.5m);

            var result = new List<Content>();
            foreach (var item in content)
            {
                var post = await postRepo.GetByIdAsync(item.ContentId);
                if (post != null)
                {
                    result.Add(new Content
                    {
                        ContentId = post.Id,
                        Value = post.Content,
                        CreatedAt = post.CreatedAt
                    });
                }
            }

            return result;
        }

        private async Task<string> GetContentByIdAsync(Guid contentId)
        {
            var post = await postRepo.GetByIdAsync(contentId);
            return post?.Content ?? string.Empty;
        }

        private async Task<List<Content>> GetRecentContentAsync(int count)
        {
            var posts = await postRepo.GetRecentPostsAsync(count);
            return posts.Select(p => new Content
            {
                ContentId = p.Id,
                Value = p.Content,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        private async Task<List<TrendingContent>> GetTrendingContentAsync()
        {
            // var trending = await trendingRepo.GetTrendingContentAsync(
            //    limit: 50,
            //    timeWindow: "day",
            //    minTrendingScore: 0.3m);

            // return trending.Select(t => new TrendingContent
            // {
            //    ContentId = t.ContentId,
            //    TrendingScore = t.TrendingScore
            // }).ToList();
            // TODO: Create repository function for this.
            throw new NotImplementedException();
        }

        private decimal CalculatePreferenceSimilarity(
            Dictionary<string, decimal> prefs1,
            Dictionary<string, decimal> prefs2)
        {
            var topics = prefs1.Keys.Union(prefs2.Keys);
            var dotProduct = (double)topics.Sum(t =>
                (prefs1.ContainsKey(t) ? prefs1[t] : 0) *
                (prefs2.ContainsKey(t) ? prefs2[t] : 0));

            var norm1 = Math.Sqrt((double)prefs1.Values.Sum(v => v * v));
            var norm2 = Math.Sqrt((double)prefs2.Values.Sum(v => v * v));

            return (decimal)(dotProduct / (norm1 * norm2));
        }

        Task<List<Core.Interfaces.ContentRecommendation>> IRecommendationService.GetRecommendationsAsync(Guid userId, List<UserInteractionHistory> userHistory, int count, List<string> excludedContentIds)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
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