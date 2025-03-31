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
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RecommendationBackgroundService> _logger;
        private readonly TimeSpan _trendingUpdateInterval = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _similarityUpdateInterval = TimeSpan.FromHours(1);

        public RecommendationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<RecommendationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
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
                    await Task.Delay(_trendingUpdateInterval, stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error updating recommendations");
                    // Wait a shorter interval before retrying after error
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task UpdateTrendingContentAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
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
                if (stoppingToken.IsCancellationRequested) return;

                try
                {
                    await trendingRepo.RecalculateTrendingScoresAsync(window);
                    _logger.LogInformation("Updated trending content for {Window} window", window);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating trending content for {Window} window", window);
                }
            }
        }

        private async Task UpdateContentSimilaritiesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var similarityRepo = scope.ServiceProvider.GetRequiredService<ContentSimilarityRepository>();
            var threadAnalyzer = scope.ServiceProvider.GetRequiredService<IThreadAnalyzer>();

            try
            {
                // Get content that needs similarity updates
                // TODO: Implement a way to track which content needs updates
                var contentPairsToUpdate = await GetContentPairsForUpdateAsync(scope);

                foreach (var pair in contentPairsToUpdate)
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    try
                    {
                        var similarity = await threadAnalyzer.CalculateSemanticSimilarityAsync(
                            pair.content1,
                            pair.content2);

                        var contentSimilarity = ContentSimilarity.Create(
                            pair.id1,
                            pair.id2,
                            (decimal)similarity);

                        await similarityRepo.AddAsync(contentSimilarity);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calculating similarity for content pair {Id1}, {Id2}",
                            pair.id1, pair.id2);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating content similarities");
            }
        }

        private async Task<List<(Guid id1, string content1, Guid id2, string content2)>> GetContentPairsForUpdateAsync(
            IServiceScope scope)
        {
            // TODO: Implement logic to identify content pairs that need similarity updates
            // This could be based on:
            // - New content
            // - Content with outdated similarity scores
            // - Content that has been significantly updated
            return new List<(Guid, string, Guid, string)>();
        }
    }
}