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
    using Microsoft.SemanticKernel;
    using Meritocious.AI.Search;
    using Meritocious.AI.Recommendations.Services;
    using Meritocious.AI.Recommendations;
    using Meritocious.Core.Interfaces;
    using Meritocious.AI.SemanticKernel.Interfaces;
    using Meritocious.AI.SemanticKernel.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeritociousAI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration
            services.Configure<AIServiceConfiguration>(
                configuration.GetSection("AIServices"));

            services.AddVectorRecommendations(options =>
            {
                configuration.GetSection("VectorDB").Bind(options);
            });

            // Register AI services
            services.AddScoped<IMeritScorer, MeritScoringService>();
            services.AddScoped<IContentModerator, ContentModerationService>();
            services.AddScoped<IThreadAnalyzer, ThreadAnalyzerService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<ISemanticSearchService, SemanticSearchService>();
            services.AddScoped<ISemanticKernelService, SemanticKernelService>();

            return services;
        }
    }
}