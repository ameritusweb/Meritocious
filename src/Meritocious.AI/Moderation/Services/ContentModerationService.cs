using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Meritocious.AI.Shared.Configuration;
using Meritocious.Common.Enums;
using System.Text.Json;
using Meritocious.AI.Moderation.Interfaces;
using Meritocious.Core.Features.Moderation.Models;
using Meritocious.Common.DTOs.Moderation;

namespace Meritocious.AI.Moderation.Services
{
    public class ContentModerationService : IContentModerator
    {
        private readonly IKernel _semanticKernel;
        private readonly ILogger<ContentModerationService> _logger;
        private readonly AIServiceConfiguration _config;

        public ContentModerationService(
            IKernel semanticKernel,
            IOptions<AIServiceConfiguration> config,
            ILogger<ContentModerationService> logger)
        {
            _semanticKernel = semanticKernel;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<ModerationActionType> EvaluateContentAsync(string content)
        {
            try
            {
                // 1. Get toxicity scores
                var toxicityScores = await GetToxicityScoresAsync(content);

                // 2. Check for spam patterns
                var spamScore = await DetectSpamAsync(content);

                // 3. Analyze harmful content
                var harmScore = await AnalyzeHarmfulContentAsync(content);

                // 4. Check for prohibited content
                var prohibitedScore = await CheckProhibitedContentAsync(content);

                // Determine moderation action based on scores
                if (toxicityScores["severe_toxicity"] > 0.8m ||
                    toxicityScores["threat"] > 0.8m ||
                    prohibitedScore > 0.8m)
                {
                    return ModerationActionType.Delete;
                }

                if (toxicityScores["toxicity"] > 0.7m ||
                    spamScore > 0.8m ||
                    harmScore > 0.7m)
                {
                    return ModerationActionType.Hide;
                }

                if (toxicityScores["toxicity"] > 0.5m ||
                    spamScore > 0.6m ||
                    harmScore > 0.5m)
                {
                    return ModerationActionType.Flag;
                }

                if (toxicityScores["toxicity"] > 0.3m ||
                    spamScore > 0.4m ||
                    harmScore > 0.3m)
                {
                    return ModerationActionType.RequireReview;
                }

                return ModerationActionType.None;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating content for moderation");
                return ModerationActionType.RequireReview;
            }
        }

        public async Task<Dictionary<string, decimal>> GetToxicityScoresAsync(string content)
        {
            try
            {
                // Define toxicity categories to evaluate
                var categories = new[]
                {
                    "toxicity",
                    "severe_toxicity",
                    "identity_attack",
                    "insult",
                    "threat",
                    "sexual_explicit",
                    "harassment",
                    "hate_speech"
                };

                var scores = new Dictionary<string, decimal>();

                // Create semantic functions for each category
                foreach (var category in categories)
                {
                    var prompt = $@"Evaluate this text for {category.Replace("_", " ")}.
                                 Consider intent, severity, and context.
                                 Rate from 0 to 1, where 1 is highest {category}.
                                 Provide explanation with the score.
                                 Text: {{$text}}";

                    var result = await _semanticKernel.InvokeSemanticFunctionAsync(prompt, new { text = content });
                    var analysis = JsonSerializer.Deserialize<ToxicityAnalysis>(result.ToString());
                    scores[category] = analysis.Score;
                }

                return scores;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating toxicity scores");
                return new Dictionary<string, decimal>
                {
                    ["toxicity"] = 0.5m,
                    ["severe_toxicity"] = 0.0m
                };
            }
        }

        private async Task<decimal> DetectSpamAsync(string content)
        {
            // Check common spam indicators
            var spamIndicators = new Dictionary<string, decimal>
            {
                { @"\b(buy|sell|discount|offer|price|deal|sale)\b", 0.3m },
                { @"(https?://|www\.)\S+", 0.4m },
                { @"[A-Z]{4,}", 0.3m },  // CAPS
                { @"(.)\1{3,}", 0.3m },  // Repeated characters
                { @"(?i)(viagra|casino|forex|crypto|pill|diet|weight.*loss)", 0.5m }
            };

            decimal spamScore = 0;
            foreach (var indicator in spamIndicators)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(content, indicator.Key).Count;
                if (matches > 0)
                {
                    spamScore += indicator.Value * Math.Min(1.0m, matches * 0.2m);
                }
            }

            // Use semantic kernel for context-aware spam detection
            var spamPrompt = @"Analyze this text for spam characteristics.
                           Consider:
                           1. Commercial intent
                           2. Repetitive content
                           3. Irrelevant links
                           4. Misleading claims
                           
                           Rate from 0-1 and explain why.
                           Text: {{$text}}";

            var result = await _semanticKernel.InvokeSemanticFunctionAsync(spamPrompt, new { text = content });
            var aiSpamScore = decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;

            // Combine both scores
            return Math.Min(1.0m, (spamScore * 0.4m + aiSpamScore * 0.6m));
        }

        private async Task<decimal> AnalyzeHarmfulContentAsync(string content)
        {
            // Define categories of harmful content to check
            var harmCategories = new[]
            {
                "misinformation",
                "personal_attacks",
                "harassment",
                "hate_speech",
                "extremism",
                "self_harm",
                "violence"
            };

            decimal totalScore = 0;
            var analysisResults = new List<HarmAnalysis>();

            foreach (var category in harmCategories)
            {
                var prompt = $@"Analyze this text for {category.Replace("_", " ")}.
                             Consider:
                             1. Intent and severity
                             2. Potential harm to individuals or groups
                             3. Context and implications
                             4. Factual accuracy (for misinformation)
                             
                             Rate from 0-1 and provide detailed explanation.
                             Text: {{$text}}";

                var result = await _semanticKernel.InvokeSemanticFunctionAsync(prompt, new { text = content });
                var analysis = JsonSerializer.Deserialize<HarmAnalysis>(result.ToString());

                analysisResults.Add(analysis);
                totalScore += analysis.Score * analysis.Weight;
            }

            // Calculate weighted average
            decimal weightSum = analysisResults.Sum(a => a.Weight);
            return totalScore / weightSum;
        }

        private async Task<decimal> CheckProhibitedContentAsync(string content)
        {
            // Define prohibited content patterns
            var prohibitedPatterns = new Dictionary<string, decimal>
            {
                // Personal Information
                { @"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b", 0.9m },  // Phone numbers
                { @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", 0.8m },  // Email addresses
                { @"\b(?:\d[ -]*?){13,16}\b", 0.9m },  // Credit card numbers
                
                // Explicit Content Markers
                { @"(?i)\b(nude|naked|sex|porn)\b", 0.7m },
                
                // Security Threats
                { @"(?i)\b(hack|crack|exploit|vulnerability)\b", 0.6m },
                
                // Financial Scams
                { @"(?i)\b(investment.*opportunity|guaranteed.*profit|bitcoin.*wallet)\b", 0.8m }
            };

            decimal maxScore = 0;
            foreach (var pattern in prohibitedPatterns)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern.Key))
                {
                    maxScore = Math.Max(maxScore, pattern.Value);
                }
            }

            // Use semantic kernel for context-aware analysis
            var prohibitedPrompt = @"Analyze this text for prohibited content.
                                 Consider:
                                 1. Personal identifiable information
                                 2. Explicit content
                                 3. Security threats
                                 4. Financial scams
                                 5. Other prohibited material
                                 
                                 Rate severity from 0-1 and explain findings.
                                 Text: {{$text}}";

            var result = await _semanticKernel.InvokeSemanticFunctionAsync(prohibitedPrompt, new { text = content });
            var aiScore = decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;

            // Return the higher of the two scores
            return Math.Max(maxScore, aiScore);
        }

        private class ToxicityAnalysis
        {
            public decimal Score { get; set; }
            public string Explanation { get; set; }
        }

        private class HarmAnalysis
        {
            public decimal Score { get; set; }
            public decimal Weight { get; set; }
            public string Category { get; set; }
            public string Explanation { get; set; }
        }
    }
}