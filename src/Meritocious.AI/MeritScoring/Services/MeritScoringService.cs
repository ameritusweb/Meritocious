using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Meritocious.AI.Shared.Configuration;
using Meritocious.Common.DTOs.Merit;
using Meritocious.Common.Extensions;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using System.Text.RegularExpressions;
using Meritocious.AI.MeritScoring.Interfaces;
using Meritocious.AI.Moderation.Interfaces;
using Meritocious.Common.Constants;
using Meritocious.AI.SemanticKernel.Interfaces;

namespace Meritocious.AI.MeritScoring.Services
{
    public class MeritScoringService : IMeritScorer
    {
        private readonly ISemanticKernelService semanticKernelService;
        private readonly ILogger<MeritScoringService> logger;
        private readonly AIServiceConfiguration config;
        private readonly IContentModerator contentModerator;

        public MeritScoringService(
            ISemanticKernelService semanticKernelService,
            IContentModerator contentModerator,
            IOptions<AIServiceConfiguration> config,
            ILogger<MeritScoringService> logger)
        {
            this.semanticKernelService = semanticKernelService;
            this.contentModerator = contentModerator;
            this.config = config.Value;
            this.logger = logger;
        }

        public async Task<MeritScoreDto> ScoreContentAsync(string content, string? context = null)
        {
            try
            {
                var scores = new Dictionary<string, decimal>();
                var explanations = new Dictionary<string, string>();

                // 1. Clarity Score
                var clarityScore = await CalculateClarityScoreAsync(content);
                scores["clarity"] = clarityScore.score;
                explanations["clarity"] = clarityScore.explanation;

                // 2. Novelty Score (comparing with context if available)
                var noveltyScore = await CalculateNoveltyScoreAsync(content, context);
                scores["novelty"] = noveltyScore.score;
                explanations["novelty"] = noveltyScore.explanation;

                // 3. Contribution Score
                var contributionScore = await CalculateContributionScoreAsync(content, context);
                scores["contribution"] = contributionScore.score;
                explanations["contribution"] = contributionScore.explanation;

                // 4. Civility Score (using content moderator)
                var civilityScore = await CalculateCivilityScoreAsync(content);
                scores["civility"] = civilityScore.score;
                explanations["civility"] = civilityScore.explanation;

                // 5. Relevance Score
                var relevanceScore = await CalculateRelevanceScoreAsync(content, context);
                scores["relevance"] = relevanceScore.score;
                explanations["relevance"] = relevanceScore.explanation;

                // Calculate final weighted score
                var finalScore = CalculateWeightedScore(scores);

                return new MeritScoreDto
                {
                    ClarityScore = scores["clarity"],
                    NoveltyScore = scores["novelty"],
                    ContributionScore = scores["contribution"],
                    CivilityScore = scores["civility"],
                    RelevanceScore = scores["relevance"],
                    FinalScore = finalScore,
                    ModelVersion = "1.0.0",
                    Explanations = explanations
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating merit score for content");
                throw;
            }
        }

        private async Task<(decimal score, string explanation)> CalculateClarityScoreAsync(string content)
        {
            // 1. Calculate readability metrics
            var readabilityMetrics = CalculateReadabilityMetrics(content);

            // 2. Check for coherent structure
            var structureScore = await EvaluateStructureAsync(content);

            // 3. Check technical accuracy (for technical content)
            var technicalScore = await EvaluateTechnicalAccuracyAsync(content);

            // 4. Grammar check
            var grammarScore = await CheckGrammarAsync(content);

            // Combine scores with weights
            decimal finalScore = (
                readabilityMetrics.score * 0.3m +
                structureScore * 0.3m +
                technicalScore.score * 0.2m +
                grammarScore.score * 0.2m);

            var explanation = $"Readability: {readabilityMetrics.explanation}. " +
                            $"Structure: {structureScore.explanation}. " +
                            $"Technical accuracy: {technicalScore.explanation}. " +
                            $"Grammar: {grammarScore.explanation}";

            return (Math.Min(1.0m, finalScore), explanation);
        }

        private async Task<(decimal score, string explanation)> CalculateNoveltyScoreAsync(
            string content,
            string? context)
        {
            // 1. Generate embeddings for the content
            var contentEmbedding = await semanticKernelService.GetEmbeddingAsync(content);

            // 2. If context exists, compare with it
            decimal similarityScore = 0;
            if (!string.IsNullOrEmpty(context))
            {
                var contextEmbedding = await semanticKernelService.GetEmbeddingAsync(context);
                similarityScore = CalculateCosineSimilarity(contentEmbedding.AsMemory(), contextEmbedding.AsMemory());
            }

            // 3. Check for unique concepts and ideas
            var uniqueConceptsScore = await AnalyzeUniqueConceptsAsync(content);

            // 4. Check information density
            var informationDensityScore = CalculateInformationDensity(content);

            // Combine scores
            decimal noveltyScore = (
                (1 - similarityScore) * 0.4m +
                uniqueConceptsScore * 0.4m +
                informationDensityScore * 0.2m);

            string explanation = $"Content originality: {(1 - similarityScore):P}. " +
                               $"Unique concepts: {uniqueConceptsScore:P}. " +
                               $"Information density: {informationDensityScore:P}";

            return (Math.Min(1.0m, noveltyScore), explanation);
        }

        private async Task<(decimal score, string explanation)> CalculateContributionScoreAsync(
            string content,
            string? context)
        {
            // 1. Analyze argument strength
            var argumentScore = await AnalyzeArgumentStrengthAsync(content);

            // 2. Check evidence and citations
            var evidenceScore = await AnalyzeEvidenceQualityAsync(content);

            // 3. Evaluate insight depth
            var insightScore = await EvaluateInsightDepthAsync(content);

            // 4. Context advancement (how much it moves the discussion forward)
            var contextAdvancementScore = !string.IsNullOrEmpty(context) ?
                await EvaluateContextAdvancementAsync(content, context) :
                0.75m; // Default score for new threads

            // Combine scores
            decimal contributionScore = (
                argumentScore.score * 0.3m +
                evidenceScore.score * 0.2m +
                insightScore.score * 0.25m +
                contextAdvancementScore * 0.25m);

            string explanation = $"Argument strength: {argumentScore.explanation}. " +
                               $"Evidence quality: {evidenceScore.explanation}. " +
                               $"Insight depth: {insightScore.explanation}. " +
                               $"Context advancement: {(contextAdvancementScore):P}";

            return (Math.Min(1.0m, contributionScore), explanation);
        }

        private async Task<(decimal score, string explanation)> CalculateCivilityScoreAsync(string content)
        {
            // 1. Get toxicity scores from content moderator
            var toxicityScores = await contentModerator.GetToxicityScoresAsync(content);

            // 2. Analyze tone and respect
            var toneScore = await AnalyzeToneAsync(content);

            // 3. Check for constructive language
            var constructiveScore = await EvaluateConstructiveLanguageAsync(content);

            // 4. Evaluate empathy
            var empathyScore = await EvaluateEmpathyAsync(content);

            // Calculate base civility (inverse of toxicity)
            decimal baseCivility = 1 - (decimal)toxicityScores["toxicity"];

            // Combine scores
            decimal civilityScore = (
                baseCivility * 0.4m +
                toneScore * 0.2m +
                constructiveScore * 0.2m +
                empathyScore * 0.2m);

            string explanation = $"Content civility: {baseCivility:P}. " +
                               $"Tone: {toneScore:P}. " +
                               $"Constructive language: {constructiveScore:P}. " +
                               $"Empathy: {empathyScore:P}";

            return (Math.Min(1.0m, civilityScore), explanation);
        }

        private async Task<(decimal score, string explanation)> CalculateRelevanceScoreAsync(
            string content,
            string? context)
        {
            if (string.IsNullOrEmpty(context))
            {
                return (1.0m, "No context provided for relevance calculation");
            }

            // 1. Calculate semantic similarity
            var contentEmbedding = await semanticKernelService.GetEmbeddingAsync(content);
            var contextEmbedding = await semanticKernelService.GetEmbeddingAsync(context);
            var semanticSimilarity = CalculateCosineSimilarity(contentEmbedding.AsMemory(), contextEmbedding.AsMemory());

            // 2. Analyze topic coherence
            var topicCoherence = await AnalyzeTopicCoherenceAsync(content, context);

            // 3. Check for contextual references
            var contextualReferences = await AnalyzeContextualReferencesAsync(content, context);

            // Combine scores
            decimal relevanceScore = (
                semanticSimilarity * 0.4m +
                topicCoherence * 0.3m +
                contextualReferences * 0.3m
            );

            string explanation = $"Semantic relevance: {semanticSimilarity:P}. " +
                               $"Topic coherence: {topicCoherence:P}. " +
                               $"Contextual references: {contextualReferences:P}";

            return (Math.Min(1.0m, relevanceScore), explanation);
        }

        private decimal CalculateWeightedScore(Dictionary<string, decimal> scores)
        {
            return scores["clarity"] * MeritScoreConstants.ClarityWeight +
                   scores["novelty"] * MeritScoreConstants.NoveltyWeight +
                   scores["contribution"] * MeritScoreConstants.ContributionWeight +
                   scores["civility"] * MeritScoreConstants.CivilityWeight +
                   scores["relevance"] * MeritScoreConstants.RelevanceWeight;
        }

        public async Task<bool> ValidateContentQualityAsync(string content, decimal minimumScore = 0.3m)
        {
            var score = await ScoreContentAsync(content);
            return score.FinalScore >= minimumScore;
        }

        #region Helper Methods

        private (decimal score, string explanation) CalculateReadabilityMetrics(string content)
        {
            // Calculate various readability metrics (Flesch-Kincaid, etc.)
            var sentences = TextChunker.SplitPlainTextLines(content, 100).Count();
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var chars = content.Length;

            // Simple readability calculation (can be enhanced)
            decimal avgWordsPerSentence = words / (decimal)Math.Max(1, sentences);
            decimal avgCharsPerWord = chars / (decimal)Math.Max(1, words);

            // Optimal ranges: 15-20 words per sentence, 4-7 chars per word
            decimal sentenceScore = Math.Max(0, 1 - Math.Abs(avgWordsPerSentence - 17.5m) / 17.5m);
            decimal wordScore = Math.Max(0, 1 - Math.Abs(avgCharsPerWord - 5.5m) / 5.5m);

            decimal score = (sentenceScore + wordScore) / 2;
            string explanation = $"Average words per sentence: {avgWordsPerSentence:F1}, " +
                               $"Average characters per word: {avgCharsPerWord:F1}";

            return (score, explanation);
        }

        private async Task<decimal> EvaluateStructureAsync(string content)
        {
            // Use semantic kernel to evaluate structure coherence
            var structurePrompt = @"Evaluate the structure coherence of this text.
                                  Consider: 1) Clear flow of ideas
                                          2) Logical paragraph organization
                                          3) Proper transitions
                                  Rate from 0 to 1, where 1 is perfectly structured.
                                  Text: {{$text}}";

            var result = await semanticKernelService.CompleteTextAsync(structurePrompt, new Dictionary<string, object> { ["text"] = content });
            return decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;
        }

        private decimal CalculateCosineSimilarity(ReadOnlyMemory<float> v1, ReadOnlyMemory<float> v2)
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

            return (decimal)(dotProduct / (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2)));
        }

        private async Task<decimal> AnalyzeUniqueConceptsAsync(string content)
        {
            var conceptsPrompt = @"Identify unique concepts and ideas in this text.
                                 Rate originality from 0 to 1, where 1 is highly original.
                                 Text: {{$text}}";

            var result = await semanticKernelService.CompleteTextAsync(conceptsPrompt, new Dictionary<string, object> { ["text"] = content });
            return decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;
        }

        private decimal CalculateInformationDensity(string content)
        {
            // Calculate ratio of meaningful words to total words
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var stopWords = new HashSet<string> { "the", "a", "an", "and", "or", "but", "in", "on", "at", "to" };

            var meaningfulWords = words.Count(w => !stopWords.Contains(w.ToLower()));
            return Math.Min(1.0m, meaningfulWords / (decimal)Math.Max(1, words.Length));
        }

        private async Task<(decimal score, string explanation)> AnalyzeArgumentStrengthAsync(string content)
        {
            var analysisPrompt = @"Analyze the argument strength in this text.
                                Consider:
                                1. Logical reasoning
                                2. Supporting evidence
                                3. Counter-argument addressing
                                4. Conclusion strength
                                
                                Rate each aspect from 0-1 and provide brief explanations.
                                Text: {{$text}}";

            // Use semantic kernel to analyze
            var result = await semanticKernelService.CompleteTextAsync(analysisPrompt, new Dictionary<string, object> { ["text"] = content });
            var analysis = JsonSerializer.Deserialize<ArgumentAnalysis>(result.ToString());

            decimal score = (
                analysis.LogicalReasoning +
                analysis.SupportingEvidence +
                analysis.CounterArgumentAddressing +
                analysis.ConclusionStrength) / 4;

            string explanation = $"Logical reasoning: {analysis.LogicalReasoningExplanation}";

            return (score, explanation);
        }

        private async Task<(decimal score, string explanation)> AnalyzeEvidenceQualityAsync(string content)
        {
            // Extract citations and references
            var citations = ExtractCitations(content);

            // Check for data and statistics
            var dataPoints = ExtractDataPoints(content);

            // Evaluate source credibility
            var sourceCredibility = await EvaluateSourceCredibilityAsync(citations);

            decimal score = citations.Any() ? 0.8m : 0.4m;
            score = Math.Min(1.0m, score + (dataPoints.Count * 0.1m));
            score = Math.Min(1.0m, score * sourceCredibility);

            string explanation = $"Found {citations.Count} citations and {dataPoints.Count} data points";

            return (score, explanation);
        }

        private async Task<decimal> EvaluateInsightDepthAsync(string content)
        {
            var insightPrompt = @"Evaluate the depth of insights in this text.
                               Consider:
                               1. Novel perspectives
                               2. Deep analysis
                               3. Practical implications
                               4. Theoretical understanding
                               
                               Rate from 0-1 and explain why.
                               Text: {{$text}}";

            var result = await semanticKernelService.CompleteTextAsync(insightPrompt, new Dictionary<string, object> { ["text"] = content });
            return decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;
        }

        private async Task<decimal> EvaluateContextAdvancementAsync(string content, string context)
        {
            // Generate embeddings for both texts
            var contentEmbedding = await semanticKernelService.GetEmbeddingAsync(content);
            var contextEmbedding = await semanticKernelService.GetEmbeddingAsync(context);

            // Calculate semantic similarity
            var similarity = CalculateCosineSimilarity(contentEmbedding, contextEmbedding);

            // Check if content builds upon context
            var buildingPrompt = @"Evaluate how this response builds upon or advances the original context.
                                Consider:
                                1. New information added
                                2. Deeper analysis
                                3. Alternative perspectives
                                4. Practical applications
                                
                                Rate from 0-1.
                                Context: {{$context}}
                                Response: {{$content}}";

            var result = await semanticKernelService.CompleteTextAsync(
                buildingPrompt,
                new Dictionary<string, object> { ["context"] = context, ["content"] = content });

            var buildingScore = decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;

            // Combine scores - we want relevant (similar) content that adds value (building score)
            return (similarity * 0.4m + buildingScore * 0.6m);
        }

        private async Task<decimal> AnalyzeToneAsync(string content)
        {
            var tonePrompt = @"Analyze the tone of this text for:
                            1. Respectfulness
                            2. Professionalism
                            3. Constructiveness
                            4. Empathy
                            
                            Rate from 0-1.
                            Text: {{$text}}";

            var result = await semanticKernelService.CompleteTextAsync(tonePrompt, new Dictionary<string, object> { ["text"] = content });
            return decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;
        }

        private async Task<decimal> EvaluateConstructiveLanguageAsync(string content)
        {
            // Check for solution-oriented language
            var solutionWords = new[] { "suggest", "recommend", "propose", "solution", "improve", "consider" };
            var constructiveMatches = solutionWords.Sum(w =>
                Regex.Matches(content.ToLower(), w).Count);

            // Use semantic kernel for deeper analysis
            var constructivePrompt = @"Evaluate how constructive this text is.
                                   Consider:
                                   1. Solution-oriented approach
                                   2. Positive suggestions
                                   3. Actionable feedback
                                   4. Balanced criticism
                                   
                                   Rate from 0-1.
                                   Text: {{$text}}";

            var result = await semanticKernelService.CompleteTextAsync(constructivePrompt, new Dictionary<string, object> { ["text"] = content });
            var aiScore = decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;

            // Combine both scores
            return (aiScore * 0.7m + Math.Min(1.0m, constructiveMatches * 0.1m) * 0.3m);
        }

        private List<string> ExtractCitations(string content)
        {
            var citations = new List<string>();

            // Match common citation patterns
            var patterns = new[]
            {
                @"\(([^)]+\d{4}[^)]+)\)",  // (Author, 2024)
                @"([^""]+et al\.,?\s+\d{4})",  // Author et al., 2024
                @"(?<=>)\s*\[(\d+)\]",  // [1] references
                @"(?<=\s|^)@[\w-]+(?=\s|$)"  // @citations
            };

            foreach (var pattern in patterns)
            {
                citations.AddRange(
                    Regex.Matches(content, pattern)
                        .Select(m => m.Groups[1].Value));
            }

            return citations.Distinct().ToList();
        }

        private List<decimal> ExtractDataPoints(string content)
        {
            var dataPoints = new List<decimal>();

            // Match numbers with optional units or percentages
            var pattern = @"(\d+\.?\d*)\s*(%|dollars|users|points)?";

            var matches = Regex.Matches(content, pattern);
            foreach (Match match in matches)
            {
                if (decimal.TryParse(match.Groups[1].Value, out var number))
                {
                    dataPoints.Add(number);
                }
            }

            return dataPoints;
        }

        private async Task<decimal> EvaluateSourceCredibilityAsync(List<string> citations)
        {
            if (!citations.Any())
            {
                return 0.5m;
            }

            var credibilityPrompt = @"Evaluate the credibility of these sources:
                                  {{$citations}}
                                  
                                  Consider:
                                  1. Academic/Professional reputation
                                  2. Recency
                                  3. Relevance to field
                                  
                                  Rate from 0-1.";

            var result = await semanticKernelService.CompleteTextAsync(
                credibilityPrompt,
                new Dictionary<string, object> { ["citations"] = string.Join("\n", citations) });

            return decimal.TryParse(result.ToString(), out var score) ? score : 0.5m;
        }

        private class ArgumentAnalysis
        {
            public decimal LogicalReasoning { get; set; }
            public string LogicalReasoningExplanation { get; set; }
            public decimal SupportingEvidence { get; set; }
            public decimal CounterArgumentAddressing { get; set; }
            public decimal ConclusionStrength { get; set; }
        }

        #endregion
    }
}