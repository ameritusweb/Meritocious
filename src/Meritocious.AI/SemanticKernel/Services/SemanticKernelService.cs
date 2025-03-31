using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Meritocious.AI.SemanticKernel.Interfaces;
using Meritocious.AI.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace Meritocious.AI.SemanticKernel.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly ILogger<SemanticKernelService> _logger;
        private readonly AIServiceConfiguration _config;
        private Kernel? _kernel;

        public SemanticKernelService(
            IOptions<AIServiceConfiguration> config,
            ILogger<SemanticKernelService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        private Kernel GetKernel()
        {
            if (_kernel != null)
                return _kernel;

            var builder = Kernel.CreateBuilder();

            // Embedding configuration
            var embeddings = _config.Embeddings;
            if (embeddings != null && embeddings.TryGetValue("ApiKey", out var embeddingApiKey))
            {
                var embeddingModel = embeddings.TryGetValue("ModelId", out var model)
                    ? model
                    : "text-embedding-ada-002";

#pragma warning disable SKEXP0010

                builder.AddOpenAITextEmbeddingGeneration(embeddingModel, embeddingApiKey);
            }
            else
            {
                throw new InvalidOperationException("Embedding API key not configured.");
            }

            // Completion configuration
            var completion = _config.Completion;
            if (completion != null && completion.TryGetValue("ApiKey", out var completionApiKey))
            {
                var completionModel = completion.TryGetValue("ModelId", out var model)
                    ? model
                    : "gpt-3.5-turbo";

                builder.AddOpenAIChatCompletion(completionModel, completionApiKey);
            }
            else
            {
                throw new InvalidOperationException("Completion API key not configured.");
            }

            _kernel = builder.Build();
            return _kernel;
        }

        public async Task<float[]> GetEmbeddingAsync(string input)
        {
            try
            {

#pragma warning disable SKEXP0001

                var kernel = GetKernel();
                var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
                var embedding = await embeddingService.GenerateEmbeddingAsync(input);
                return embedding.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding.");
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
                var kernel = GetKernel();
                var function = kernel.CreateFunctionFromPrompt(prompt);
                var arguments = new KernelArguments(variables, settings);

                var result = await kernel.InvokeAsync(function, arguments);

                return result.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invoke prompt with settings.");
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
