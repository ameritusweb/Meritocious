using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Meritocious.AI.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Meritocious.AI.SemanticClustering.Interfaces;

namespace Meritocious.AI.SemanticClustering.Services
{
    public class SemanticClusteringService : ISemanticClusteringService
    {
        private readonly ILogger<SemanticClusteringService> _logger;
        private readonly AIServiceConfiguration _config;
        private readonly IThreadAnalyzer _threadAnalyzer;

        public SemanticClusteringService(
            ILogger<SemanticClusteringService> logger,
            IOptions<AIServiceConfiguration> config,
            IThreadAnalyzer threadAnalyzer)
        {
            _logger = logger;
            _config = config.Value;
            _threadAnalyzer = threadAnalyzer;
        }

        public async Task<List<string>> IdentifyTopicsAsync(List<string> contents)
        {
            if (contents == null || !contents.Any())
                return new List<string>();

            var allTopics = new List<string>();

            // Extract topics from each content
            foreach (var content in contents)
            {
                var topics = await _threadAnalyzer.ExtractKeyTopicsAsync(content);
                allTopics.AddRange(topics);
            }

            // Group similar topics
            var groupedTopics = new Dictionary<string, List<string>>();
            foreach (var topic in allTopics.Distinct())
            {
                bool added = false;
                foreach (var group in groupedTopics.Keys.ToList())
                {
                    if (await IsSimilarTopic(topic, group))
                    {
                        groupedTopics[group].Add(topic);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    groupedTopics[topic] = new List<string> { topic };
                }
            }

            // Select representative topic from each group
            return groupedTopics.Select(g => g.Value.First()).ToList();
        }

        private async Task<bool> IsSimilarTopic(string topic1, string topic2)
        {
            var similarity = await _threadAnalyzer.CalculateSemanticSimilarityAsync(topic1, topic2);
            return similarity > 0.7; // Threshold for similarity
        }

        public async Task<List<string>> GetRelatedTagsAsync(string topic)
        {
            // This would be replaced with actual tag recommendation logic
            var topics = await _threadAnalyzer.ExtractKeyTopicsAsync(topic + " related concepts associated terms");
            return topics.Take(5).ToList();
        }
    }
}