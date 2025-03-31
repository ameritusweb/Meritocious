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

            // Add Semantic Kernel
            services.AddSingleton<IKernelBuilder>(sp =>
            {
                var builder = Kernel.CreateBuilder();

                // Configure the embedding model
                // Replace with your actual model configuration
                var config = configuration.GetSection("AIServices:Embeddings").Get<Dictionary<string, string>>();
                var apiKey = config?["ApiKey"] ?? throw new InvalidOperationException("Embedding API key not found");
                var modelId = config?["ModelId"] ?? "text-embedding-ada-002";

                builder.AddOpenAITextEmbeddingGeneration(modelId, apiKey);

                // Configure the text completion model for prompts
                var completionConfig = configuration.GetSection("AIServices:Completion").Get<Dictionary<string, string>>();
                var completionApiKey = completionConfig?["ApiKey"] ?? apiKey;
                var completionModelId = completionConfig?["ModelId"] ?? "gpt-3.5-turbo";

                builder.AddOpenAIChatCompletion(completionModelId, completionApiKey);

                return builder;
            });

            // Register AI services
            services.AddScoped<IMeritScorer, MeritScoringService>();
            services.AddScoped<IContentModerator, ContentModerationService>();
            services.AddScoped<IThreadAnalyzer, ThreadAnalyzerService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<ISemanticSearchService, SemanticSearchService>();

            return services;
        }
    }
}