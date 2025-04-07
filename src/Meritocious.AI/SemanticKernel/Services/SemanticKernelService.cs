using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Meritocious.AI.SemanticKernel.Interfaces;
using Meritocious.AI.Shared.Configuration;
using Microsoft.Extensions.Options;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Constants;

namespace Meritocious.AI.SemanticKernel.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly ILogger<SemanticKernelService> logger;
        private readonly AIServiceConfiguration config;
        private readonly ISecretsService secretsService;
        private Kernel? kernel;

        public SemanticKernelService(
            IOptions<AIServiceConfiguration> config,
            ISecretsService secretsService,
            ILogger<SemanticKernelService> logger)
        {
            this.config = config.Value;
            this.secretsService = secretsService;
            this.logger = logger;
        }

        private async Task<Kernel> GetKernel()
        {
            if (kernel != null)
            {
                return kernel;
            }

            var builder = Kernel.CreateBuilder();

            // Embedding configuration
            var embeddingModel = config.Embeddings?.TryGetValue("ModelId", out var model) == true
                ? model
                : "text-embedding-ada-002";

            var embeddingApiKey = await secretsService.GetSecretAsync(SecretNames.OpenAIEmbeddingKey);

#pragma warning disable SKEXP0010
            builder.AddOpenAITextEmbeddingGeneration(embeddingModel, embeddingApiKey);

            // Completion configuration
            var completionModel = config.Completion?.TryGetValue("ModelId", out var cModel) == true
                ? cModel
                : "gpt-3.5-turbo";

            var completionApiKey = await secretsService.GetSecretAsync(SecretNames.OpenAICompletionKey);
            builder.AddOpenAIChatCompletion(completionModel, completionApiKey);

            kernel = builder.Build();
            return kernel;
        }

        public async Task<float[]> GetEmbeddingAsync(string input)
        {
            try
            {
#pragma warning disable SKEXP0001

                var kernel = await GetKernel();
                var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
                var embedding = await embeddingService.GenerateEmbeddingAsync(input);
                return embedding.ToArray();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate embedding.");
                return Array.Empty<float>();
            }
        }

        public async Task<string> CompleteTextAsync(
    string prompt,
    Dictionary<string, object>? variables = null,
    Dictionary<string, PromptExecutionSettings>? settings = null)
        {
            try
            {
                var kernel = await GetKernel();
                var function = kernel.CreateFunctionFromPrompt(prompt);
                var arguments = new KernelArguments(variables, settings);

                var result = await kernel.InvokeAsync(function, arguments);

                return result.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to invoke prompt with settings.");
                return string.Empty;
            }
        }

        public PromptExecutionSettings DefaultGpt4Settings(double temp = 0.2, int maxTokens = 300)
        {
            return new PromptExecutionSettings
            {
                ModelId = "gpt-4",
                ServiceId = "openai",
                ExtensionData = new Dictionary<string, object>
                {
                    ["temperature"] = temp,
                    ["max_tokens"] = maxTokens
                }
            };
        }
    }
}
