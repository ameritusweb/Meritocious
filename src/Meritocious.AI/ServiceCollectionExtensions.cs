
namespace Meritocious.AI
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.AI.MeritScoring.Services;
    using Meritocious.AI.Moderation.Interfaces;
    using Meritocious.AI.SemanticClustering.Interfaces;
    using Meritocious.AI.Shared.Configuration;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeritociousAI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration
            // services.Configure<AIServiceConfiguration>( // TODO: Fix this
            //     configuration.GetSection("AIServices"));

            // Register AI services
            services.AddScoped<IMeritScorer, MeritScoringService>();
            // TODO: Add other AI service implementations as they are developed

            return services;
        }
    }
}