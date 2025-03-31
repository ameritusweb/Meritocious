namespace Meritocious.AI
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.AI.MeritScoring.Services;
    using Meritocious.AI.Moderation.Interfaces;
    using Meritocious.AI.SemanticClustering.Interfaces;
    using Meritocious.AI.Shared.Configuration;
    using Meritocious.AI.Moderation.Services;
    using Meritocious.AI.SemanticClustering.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeritociousAI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration
            services.Configure<AIServiceConfiguration>(
                configuration.GetSection("AIServices"));

            // Register AI services
            services.AddScoped<IMeritScorer, MeritScoringService>();
            services.AddScoped<IContentModerator, ContentModerationService>();
            services.AddScoped<IThreadAnalyzer, ThreadAnalyzerService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<ISemanticClusteringService, SemanticClusteringService>();

            return services;
        }
    }
}