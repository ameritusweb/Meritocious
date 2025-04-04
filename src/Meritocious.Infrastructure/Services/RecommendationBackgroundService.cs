using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.AI.Recommendations.Services;
using Meritocious.AI.SemanticClustering.Interfaces;

namespace Meritocious.Infrastructure.Services
{
    public class RecommendationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RecommendationBackgroundService> logger;
        private readonly TimeSpan trendingUpdateInterval = TimeSpan.FromMinutes(15);
        private readonly TimeSpan similarityUpdateInterval = TimeSpan.FromHours(1);

        public RecommendationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<RecommendationBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Update trending content
                    await UpdateTrendingContentAsync(stoppingToken);

                    // Update content similarities if needed
                    await UpdateContentSimilaritiesAsync(stoppingToken);

                    // Wait for the next update interval
                    await Task.Delay(trendingUpdateInterval, stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Error updating recommendations");

                    // Wait a shorter interval before retrying after error
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task UpdateTrendingContentAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var trendingRepo = scope.ServiceProvider.GetRequiredService<TrendingContentRepository>();

            // Update trending scores for different time windows
            var windows = new[]
            {
                TimeSpan.FromHours(1),    // Hourly trending
                TimeSpan.FromHours(24),   // Daily trending
                TimeSpan.FromDays(7)      // Weekly trending
            };

            foreach (var window in windows)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    await trendingRepo.RecalculateTrendingScoresAsync(window);
                    logger.LogInformation("Updated trending content for {Window} window", window);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error updating trending content for {Window} window", window);
                }
            }
        }

        private async Task UpdateContentSimilaritiesAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var similarityRepo = scope.ServiceProvider.GetRequiredService<ContentSimilarityRepository>();
            var threadAnalyzer = scope.ServiceProvider.GetRequiredService<IThreadAnalyzer>();

            try
            {
                // Get content that needs similarity updates
                // Get pairs needing update (max 100 at a time)
                var contentPairs = await similarityRepo.GetContentPairsForUpdateAsync(100);
                var postRepo = scope.ServiceProvider.GetRequiredService<IPostRepository>();

                foreach (var pair in contentPairs)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        // Get the content for both posts
                        var post1 = await postRepo.GetByIdAsync(pair.id1);
                        var post2 = await postRepo.GetByIdAsync(pair.id2);

                        if (post1 == null || post2 == null)
                        {
                            logger.LogWarning("One or both posts not found for pair {Id1}, {Id2}", pair.id1, pair.id2);
                            continue;
                        }

                        // Calculate new similarity score
                        var similarity = await threadAnalyzer.CalculateSemanticSimilarityAsync(
                            post1.Content,
                            post2.Content);

                        // Update the similarity record
                        var record = await similarityRepo.DbSet
                            .FirstOrDefaultAsync(s => 
                                (s.ContentId1 == pair.id1 && s.ContentId2 == pair.id2) ||
                                (s.ContentId1 == pair.id2 && s.ContentId2 == pair.id1));

                        if (record != null)
                        {
                            record.UpdateScore((decimal)similarity);
                            await similarityRepo.DbContext.SaveChangesAsync();
                            
                            logger.LogInformation(
                                "Updated similarity score for content pair {Id1}, {Id2} to {Score}",
                                pair.id1, pair.id2, similarity);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error calculating similarity for content pair {Id1}, {Id2}",
                            pair.id1, pair.id2);
                    }
                }

                // Check for old similarity scores (older than 30 days)
                await similarityRepo.MarkOldSimilaritiesForUpdateAsync(
                    TimeSpan.FromDays(30),
                    priority: 1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating content similarities");
            }
        }

        private async Task<List<(Guid id1, string content1, Guid id2, string content2)>> GetContentPairsForUpdateAsync(
            IServiceScope scope)
        {
            var postRepo = scope.ServiceProvider.GetRequiredService<PostRepository>();
            var similarityRepo = scope.ServiceProvider.GetRequiredService<ContentSimilarityRepository>();
            var results = new List<(Guid id1, string content1, Guid id2, string content2)>();

            try
            {
                // Get content modified in last 24 hours
                var recentPosts = await postRepo.GetPostsAfterDateAsync(DateTime.UtcNow.AddHours(-24));
                
                if (recentPosts.Any())
                {
                    logger.LogInformation("Found {Count} recently modified posts to process", recentPosts.Count);
                    
                    // Create missing similarity records for new content
                    var recentIds = recentPosts.Select(p => p.Id).ToList();
                    await similarityRepo.CreateMissingSimilaritiesAsync(recentIds);

                    // Mark existing similarities involving recent posts for update
                    foreach (var post in recentPosts)
                    {
                        await similarityRepo.MarkForUpdateAsync(post.Id, priority: 2);
                    }
                }

                // Get pairs that need updating (either newly created or marked for update)
                var pairsToUpdate = await similarityRepo.GetContentPairsForUpdateAsync(100);
                
                foreach (var pair in pairsToUpdate)
                {
                    var post1 = await postRepo.GetByIdAsync(pair.id1);
                    var post2 = await postRepo.GetByIdAsync(pair.id2);

                    if (post1 != null && post2 != null && !post1.IsDeleted && !post2.IsDeleted)
                    {
                        results.Add((post1.Id, post1.Content, post2.Id, post2.Content));
                    }
                    else
                    {
                        logger.LogWarning(
                            "Skipping similarity pair ({Id1}, {Id2}) - one or both posts not found or deleted",
                            pair.id1, pair.id2);
                    }
                }

                logger.LogInformation(
                    "Retrieved {Count} content pairs for similarity update", 
                    results.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving content pairs for similarity update");
            }

            return results;
        }
    }
}