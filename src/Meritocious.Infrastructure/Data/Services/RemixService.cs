using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;
using Meritocious.AI.SemanticKernel.Interfaces;

namespace Meritocious.Infrastructure.Data.Services;

public class RemixService : IRemixService
{
    private readonly IRemixRepository _remixRepository;
    private readonly IRemixSourceRepository _sourceRepository;
    private readonly IRemixNoteRepository _noteRepository;
    private readonly IPostRepository _postRepository;
    private readonly IMeritScoringService _meritScoringService;
    private readonly ISemanticSearchService _semanticSearchService;
    private readonly ISemanticKernelService _semanticKernelService;
    private readonly ITagService _tagService;

    public RemixService(
        IRemixRepository remixRepository,
        IRemixSourceRepository sourceRepository,
        IRemixNoteRepository noteRepository,
        IPostRepository postRepository,
        IMeritScoringService meritScoringService,
        ISemanticSearchService semanticSearchService,
        ISemanticKernelService semanticKernelService,
        ITagService tagService)
    {
        _remixRepository = remixRepository;
        _sourceRepository = sourceRepository;
        _noteRepository = noteRepository;
        _postRepository = postRepository;
        _meritScoringService = meritScoringService;
        _semanticSearchService = semanticSearchService;
        _semanticKernelService = semanticKernelService;
        _tagService = tagService;
    }

    public async Task<RemixDto> CreateRemixAsync(CreateRemixRequest request)
    {
        var remix = new Remix
        {
            Title = request.Title,
            Content = request.InitialContent,
            AuthorId = request.AuthorId,
            SubstackId = request.SubstackId,
            IsDraft = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add tags
        if (request.Tags.Any())
        {
            remix.Tags = await _tagService.GetOrCreateTagsAsync(request.Tags);
        }

        // Add initial sources
        if (request.InitialSourceIds.Any())
        {
            var posts = await _postRepository.GetByIdsAsync(request.InitialSourceIds);
            remix.Sources = posts.Select(p => new RemixSource
            {
                SourcePost = p,
                SourcePostId = p.Id,
                Relationship = "support", // Default relationship
                Order = remix.Sources.Count
            }).ToList();
        }

        await _remixRepository.AddAsync(remix);

        // Generate initial synthesis map and notes
        if (remix.Sources.Any())
        {
            remix.SynthesisMap = await GenerateSynthesisMapAsync(remix.Id);
            var notes = await GenerateInsightsAsync(remix.Id);
        }

        return await GetRemixByIdAsync(remix.Id);
    }

    public async Task<RemixDto> UpdateRemixAsync(Guid remixId, UpdateRemixRequest request)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        remix.Title = request.Title;
        remix.Content = request.Content;
        remix.UpdatedAt = DateTime.UtcNow;

        // Update tags
        if (request.Tags != null)
        {
            remix.Tags = await _tagService.GetOrCreateTagsAsync(request.Tags);
        }

        // Update synthesis map if requested
        if (request.UpdateSynthesisMap)
        {
            remix.SynthesisMap = await GenerateSynthesisMapAsync(remixId);
        }

        await _remixRepository.UpdateAsync(remix);

        return await GetRemixByIdAsync(remixId);
    }

    public async Task<RemixDto> GetRemixByIdAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        return new RemixDto
        {
            Id = remix.Id,
            Title = remix.Title,
            Content = remix.Content,
            AuthorUsername = remix.Author.Username,
            AuthorId = remix.AuthorId,
            MeritScore = remix.MeritScore,
            Sources = remix.Sources.Select(MapRemixSource).ToList(),
            Tags = remix.Tags.Select(t => t.Name).ToList(),
            SubstackId = remix.SubstackId,
            IsDraft = remix.IsDraft,
            PublishedAt = remix.PublishedAt,
            CreatedAt = remix.CreatedAt,
            UpdatedAt = remix.UpdatedAt,
            Notes = remix.Notes.Select(MapRemixNote).ToList()
        };
    }

    public async Task<bool> DeleteRemixAsync(Guid remixId, Guid userId)
    {
        var remix = await _remixRepository.GetByIdAsync(remixId);
        if (remix == null || remix.AuthorId != userId) return false;

        await _remixRepository.DeleteAsync(remix);
        return true;
    }

    public async Task<RemixDto> PublishRemixAsync(Guid remixId, Guid userId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null || remix.AuthorId != userId) 
            throw new ArgumentException("Remix not found or unauthorized");

        // Calculate final merit score
        var scoreResult = await CalculateRemixScoreAsync(remixId);
        remix.MeritScore = scoreResult.FinalScore;

        // Update status
        remix.IsDraft = false;
        remix.PublishedAt = DateTime.UtcNow;
        
        await _remixRepository.UpdateAsync(remix);
        return await GetRemixByIdAsync(remixId);
    }

    public async Task<RemixSourceDto> AddSourceAsync(Guid remixId, AddSourceRequest request)
    {
        var source = new RemixSource
        {
            RemixId = remixId,
            SourcePostId = request.PostId,
            Relationship = request.Relationship,
            Context = request.Context,
            QuotedExcerpts = request.InitialQuotes,
            Order = (await _sourceRepository.GetByRemixIdAsync(remixId)).Count()
        };

        await _sourceRepository.AddAsync(source);

        // Update synthesis map and generate new insights
        await UpdateSynthesisMapAsync(remixId, await GenerateSynthesisMapAsync(remixId));
        await GenerateInsightsAsync(remixId);

        return MapRemixSource(await _sourceRepository.GetByIdWithDetailsAsync(source.Id));
    }

    public async Task<bool> RemoveSourceAsync(Guid remixId, Guid sourceId)
    {
        var source = await _sourceRepository.GetByIdAsync(sourceId);
        if (source == null || source.RemixId != remixId) return false;

        await _sourceRepository.DeleteAsync(source);

        // Update synthesis map
        await UpdateSynthesisMapAsync(remixId, await GenerateSynthesisMapAsync(remixId));

        return true;
    }

    public async Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<SourceOrderUpdate> updates)
    {
        var orderUpdates = updates.Select(u => (u.SourceId, u.NewOrder));
        await _remixRepository.UpdateSourceOrderAsync(remixId, orderUpdates);
    }

    public async Task<RemixSourceDto> UpdateSourceRelationshipAsync(Guid remixId, Guid sourceId, string relationship)
    {
        var source = await _sourceRepository.GetByIdWithDetailsAsync(sourceId);
        if (source == null || source.RemixId != remixId) 
            throw new ArgumentException("Source not found");

        source.Relationship = relationship;
        await _sourceRepository.UpdateAsync(source);

        // Update synthesis map
        await UpdateSynthesisMapAsync(remixId, await GenerateSynthesisMapAsync(remixId));

        return MapRemixSource(source);
    }

    public async Task AddQuoteToSourceAsync(Guid sourceId, AddQuoteRequest request)
    {
        var source = await _sourceRepository.GetByIdWithDetailsAsync(sourceId);
        if (source == null) throw new ArgumentException("Source not found");

        var post = await _postRepository.GetByIdAsync(source.SourcePostId);
        if (post == null) throw new ArgumentException("Source post not found");

        // Find the quote in the source content
        var startIndex = post.Content.IndexOf(request.Text);
        if (startIndex == -1) throw new ArgumentException("Quote not found in source content");

        // Get some context around the quote
        var contextStart = Math.Max(0, startIndex - 100);
        var contextLength = Math.Min(startIndex + request.Text.Length + 100, post.Content.Length) - contextStart;
        var context = post.Content.Substring(contextStart, contextLength);

        // Create quote location
        var quote = new QuoteLocation
        {
            RemixSourceId = sourceId,
            Text = request.Text,
            StartPosition = startIndex,
            EndPosition = startIndex + request.Text.Length,
            Context = context
        };

        source.Quotes.Add(quote);
        await _sourceRepository.UpdateAsync(source);
    }

    public async Task<IEnumerable<RemixDto>> GetUserRemixesAsync(Guid userId, RemixFilter filter)
    {
        var remixes = await _remixRepository.GetUserRemixesAsync(userId, filter.IncludeDrafts);
        
        // Apply additional filters
        var filtered = remixes.Where(r => 
            (!filter.FromDate.HasValue || r.CreatedAt >= filter.FromDate) &&
            (!filter.ToDate.HasValue || r.CreatedAt <= filter.ToDate) &&
            (!filter.Tags.Any() || r.Tags.Any(t => filter.Tags.Contains(t.Name)))
        );

        // Apply sorting
        filtered = filter.SortBy switch
        {
            "merit" => filtered.OrderByDescending(r => r.MeritScore),
            "oldest" => filtered.OrderBy(r => r.CreatedAt),
            _ => filtered.OrderByDescending(r => r.CreatedAt) // "newest" is default
        };

        return filtered.Select(r => new RemixDto
        {
            Id = r.Id,
            Title = r.Title,
            AuthorUsername = r.Author.Username,
            MeritScore = r.MeritScore,
            Tags = r.Tags.Select(t => t.Name).ToList(),
            CreatedAt = r.CreatedAt,
            IsDraft = r.IsDraft
        });
    }

    public async Task<IEnumerable<RemixDto>> GetRelatedRemixesAsync(Guid remixId, int limit = 5)
    {
        var related = await _remixRepository.GetRelatedRemixesAsync(remixId, limit);
        return related.Select(r => new RemixDto
        {
            Id = r.Id,
            Title = r.Title,
            AuthorUsername = r.Author.Username,
            MeritScore = r.MeritScore,
            Tags = r.Tags.Select(t => t.Name).ToList()
        });
    }

    public async Task<IEnumerable<RemixDto>> GetTrendingRemixesAsync(int limit = 10)
    {
        var trending = await _remixRepository.GetTrendingRemixesAsync(limit);
        return trending.Select(r => new RemixDto
        {
            Id = r.Id,
            Title = r.Title,
            AuthorUsername = r.Author.Username,
            MeritScore = r.MeritScore,
            Tags = r.Tags.Select(t => t.Name).ToList()
        });
    }

    public async Task RecordEngagementAsync(
        Guid remixId,
        RemixEngagementEvent engagementEvent)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        var engagement = await _remixRepository.GetEngagementAsync(remixId);

        switch (engagementEvent.Type)
        {
            case RemixEngagementType.View:
                engagement.RecordView(
                    engagementEvent.Region ?? "unknown",
                    engagementEvent.Platform ?? "unknown");
                break;

            case RemixEngagementType.Interaction:
                engagement.RecordInteraction(
                    Enum.Parse<RemixInteractionType>(engagementEvent.InteractionType));
                break;

            case RemixEngagementType.SessionEnd:
                engagement.UpdateEngagementMetrics(
                    engagementEvent.TimeSpentSeconds,
                    engagementEvent.Bounced);
                break;

            case RemixEngagementType.Citation:
                engagement.CitationCount++;
                break;

            case RemixEngagementType.Reference:
                engagement.ReferenceCount++;
                break;
        }

        // Update source influence if provided
        if (engagementEvent.SourceId.HasValue && engagementEvent.InfluenceScore.HasValue)
        {
            engagement.UpdateSourceInfluence(
                engagementEvent.SourceId.Value,
                engagementEvent.InfluenceScore.Value);
        }

        // Update sentiment if provided
        if (engagementEvent.SentimentScore.HasValue)
        {
            var oldScore = engagement.SentimentScore * (engagement.Comments - 1);
            engagement.SentimentScore = 
                (oldScore + engagementEvent.SentimentScore.Value) / engagement.Comments;
        }

        await _remixRepository.UpdateEngagementAsync(engagement);
    }

    public async Task<RemixAnalytics> GetRemixAnalyticsAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        return new RemixAnalytics
        {
            TotalSources = remix.Sources.Count,
            RelationshipDistribution = await _sourceRepository.GetRelationshipDistributionAsync(remixId),
            NoteTypeDistribution = await _noteRepository.GetNoteTypeDistributionAsync(remixId),
            AverageSourceRelevance = remix.Sources.Average(s => 
                s.RelevanceScores.Values.DefaultIfEmpty(0).Average()),
            TopTags = remix.Tags.Select(t => t.Name).ToList(),
            Engagement = new RemixEngagementMetrics
            {
                Views = engagement.TotalViews,
                UniqueViews = engagement.UniqueViews,
                Likes = engagement.Likes,
                Comments = engagement.Comments,
                Forks = engagement.Forks,
                Shares = engagement.Shares,
                
                AverageTimeSpentMinutes = engagement.AverageTimeSpentSeconds / 60m,
                BounceRate = engagement.BounceRate,
                ContributorCount = engagement.ContributorCount,
                
                CitationCount = engagement.CitationCount,
                ReferenceCount = engagement.ReferenceCount,
                
                ViewsByRegion = engagement.ViewsByRegion,
                ViewsByPlatform = engagement.ViewsByPlatform,
                ViewTrend = engagement.ViewsOverTime,
                SourceInfluenceScores = engagement.SourceInfluenceScores
                    .ToDictionary(
                        kvp => remix.Sources
                            .First(s => s.SourcePostId == kvp.Key)
                            .SourcePost.Title,
                        kvp => kvp.Value
                    ),
                
                PeakEngagementTime = engagement.PeakEngagementTime,
                EngagementVelocity = engagement.EngagementVelocity,
                SentimentScore = engagement.SentimentScore,
                TopEngagementSources = remix.Sources
                    .OrderByDescending(s => engagement.SourceInfluenceScores
                        .GetValueOrDefault(s.SourcePostId))
                    .Take(3)
                    .Select(s => s.SourcePost.Title)
                    .ToList()
            }
        };
    }

    public async Task<IEnumerable<RemixDto>> SearchRemixesAsync(RemixSearchRequest request)
    {
        // Use semantic search for better results
        var searchResults = await _semanticSearchService.SearchAsync(
            request.Query,
            entityType: "remix",
            filters: new Dictionary<string, object>
            {
                { "tags", request.Tags },
                { "minMeritScore", request.MinMeritScore }
            }
        );

        var remixIds = searchResults.Select(r => Guid.Parse(r.Id)).ToList();
        var remixes = await _remixRepository.GetByIdsAsync(remixIds);

        // Sort based on request
        var ordered = request.SortBy switch
        {
            "merit" => remixes.OrderByDescending(r => r.MeritScore),
            "newest" => remixes.OrderByDescending(r => r.CreatedAt),
            "oldest" => remixes.OrderBy(r => r.CreatedAt),
            _ => remixes.OrderBy(r => searchResults.FindIndex(sr => 
                sr.Id == r.Id.ToString())) // Maintain semantic search order
        };

        return ordered.Select(r => new RemixDto
        {
            Id = r.Id,
            Title = r.Title,
            AuthorUsername = r.Author.Username,
            MeritScore = r.MeritScore,
            Tags = r.Tags.Select(t => t.Name).ToList(),
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<IEnumerable<RemixNoteDto>> GenerateInsightsAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        // Get source contents
        var sources = await Task.WhenAll(remix.Sources.Select(async s => 
        {
            var post = await _postRepository.GetByIdAsync(s.SourcePostId);
            return new
            {
                Id = s.SourcePostId,
                Content = post.Content,
                Title = post.Title,
                Relationship = s.Relationship
            };
        }));

        var notes = new List<RemixNote>();
        
        // 1. Generate connections between sources
        if (sources.Length > 1)
        {
            foreach (var source1 in sources)
            {
                foreach (var source2 in sources.Where(s => s.Id != source1.Id))
                {
                    var prompt = $"""
                        Analyze the relationship between these two pieces of content and identify key connections.
                        Source 1 Title: {source1.Title}
                        Source 1 Content: {source1.Content}
                        Source 2 Title: {source2.Title}
                        Source 2 Content: {source2.Content}
                        Relationship type: {source1.Relationship} -> {source2.Relationship}

                        Format your response in 1-2 clear, concise sentences that identify a specific connection,
                        common theme, or interesting contrast between these sources. Focus on substance rather
                        than surface-level similarities.
                        """;

                    var connection = await _semanticKernelService.CompleteTextAsync(prompt);
                    var confidence = CalculateConfidence(connection);

                    if (confidence > 0.7m)
                    {
                        notes.Add(new RemixNote
                        {
                            RemixId = remixId,
                            Type = "Connection",
                            Content = connection,
                            RelatedSourceIds = new List<Guid> { source1.Id, source2.Id },
                            Confidence = confidence,
                            IsApplied = false
                        });
                    }
                }
            }
        }

        // 2. Generate insights for each source
        foreach (var source in sources)
        {
            var prompt = $"""
                Analyze this content and identify a key insight that could be valuable for synthesis:
                Title: {source.Title}
                Content: {source.Content}
                Role: {source.Relationship}

                Focus on:
                - Underlying assumptions or implications
                - Unique perspectives or approaches
                - Potential applications or extensions
                - Limitations or edge cases

                Format your response as a single clear, actionable insight in 1-2 sentences.
                """;

            var insight = await _semanticKernelService.CompleteTextAsync(prompt);
            var confidence = CalculateConfidence(insight);

            if (confidence > 0.7m)
            {
                notes.Add(new RemixNote
                {
                    RemixId = remixId,
                    Type = "Insight",
                    Content = insight,
                    RelatedSourceIds = new List<Guid> { source.Id },
                    Confidence = confidence,
                    IsApplied = false
                });
            }
        }

        // 3. Generate synthesis suggestions based on all sources
        if (sources.Any())
        {
            var sourcesContext = string.Join("\n\n", sources.Select(s => 
                $"Source: {s.Title}\nRole: {s.Relationship}\nContent: {s.Content}"));

            var prompt = $"""
                Review these sources that are being synthesized:
                {sourcesContext}

                Current remix content:
                {remix.Content}

                Suggest 2-3 specific ways to strengthen the synthesis. Consider:
                - Unexplored connections between sources
                - Missing perspectives or counterarguments
                - Opportunities for deeper analysis
                - Areas that could benefit from more evidence or examples

                Format each suggestion as a clear, actionable recommendation in 1-2 sentences.
                """;

            var suggestions = await _semanticKernelService.CompleteTextAsync(prompt);
            foreach (var suggestion in suggestions.Split("\n", StringSplitOptions.RemoveEmptyEntries))
            {
                var confidence = CalculateConfidence(suggestion);
                if (confidence > 0.7m)
                {
                    notes.Add(new RemixNote
                    {
                        RemixId = remixId,
                        Type = "Suggestion",
                        Content = suggestion.Trim(),
                        RelatedSourceIds = sources.Select(s => s.Id).ToList(),
                        Confidence = confidence,
                        IsApplied = false
                    });
                }
            }
        }

        await _noteRepository.AddRangeAsync(notes);
        return notes.Select(MapRemixNote);
    }

    private decimal CalculateConfidence(string text)
    {
        // Base confidence on factors like:
        // - Specificity (avoid vague language)
        // - Actionability (clear recommendations)
        // - Relevance (uses domain terminology)
        // - Coherence (well-structured insights)
        
        decimal confidence = 1.0m;

        // Penalize vague language
        var vagueTerms = new[] { "maybe", "perhaps", "might", "could", "possibly" };
        confidence -= 0.1m * vagueTerms.Count(t => text.Contains(t));

        // Penalize very short or very long insights
        var wordCount = text.Split(' ').Length;
        if (wordCount < 10) confidence -= 0.2m;
        if (wordCount > 50) confidence -= 0.1m;

        // Penalize generic insights
        var genericPhrases = new[] { "it is important to", "one should consider", "it is interesting that" };
        confidence -= 0.15m * genericPhrases.Count(p => text.Contains(p));

        // Ensure confidence stays between 0 and 1
        return Math.Max(0.0m, Math.Min(1.0m, confidence));
    }

    public async Task<string> GenerateSynthesisMapAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        var sources = await Task.WhenAll(remix.Sources.Select(async s => 
        {
            var post = await _postRepository.GetByIdAsync(s.SourcePostId);
            return new
            {
                Id = s.SourcePostId.ToString(),
                Title = post.Title,
                Content = post.Content,
                Relationship = s.Relationship,
                Order = s.Order
            };
        }));

        // Generate node and link data for visualization
        var nodes = new List<object>();
        var links = new List<object>();
        var clusters = new List<object>();

        // 1. Add source nodes
        foreach (var source in sources)
        {
            nodes.Add(new
            {
                id = source.Id,
                label = source.Title,
                type = "source",
                relationship = source.Relationship,
                order = source.Order
            });
        }

        // 2. Analyze key themes and create theme clusters
        var allContent = string.Join("\n\n", sources.Select(s => s.Content));
        var themesPrompt = $"""
            Analyze these source contents and identify 3-5 key themes or concepts that connect them:
            {allContent}

            For each theme:
            - Provide a clear, concise label (2-3 words)
            - List which sources (by number, 1-based) strongly relate to this theme
            - Explain the theme's significance in 1 sentence

            Format as JSON:
            {{
                "themes": [
                    {{
                        "label": "theme name",
                        "sourceIndices": [1, 2],
                        "description": "theme description"
                    }}
                ]
            }}
            """;

        var themesResult = await _semanticKernelService.CompleteTextAsync(themesPrompt);
        var themes = System.Text.Json.JsonSerializer.Deserialize<ThemesResponse>(themesResult);

        // Add theme clusters and connections
        foreach (var theme in themes.Themes)
        {
            var clusterId = $"cluster_{Guid.NewGuid()}";
            
            clusters.Add(new
            {
                id = clusterId,
                label = theme.Label,
                description = theme.Description
            });

            // Connect sources to themes
            foreach (var sourceIndex in theme.SourceIndices)
            {
                if (sourceIndex <= sources.Length)
                {
                    var sourceId = sources[sourceIndex - 1].Id;
                    links.Add(new
                    {
                        source = sourceId,
                        target = clusterId,
                        type = "theme"
                    });
                }
            }
        }

        // 3. Analyze relationships between sources
        foreach (var source1 in sources)
        {
            foreach (var source2 in sources.Where(s => s.Id != source1.Id))
            {
                var relationshipPrompt = $"""
                    Analyze the relationship between these two sources:
                    Source 1: {source1.Content}
                    Source 2: {source2.Content}

                    Describe their relationship in a single keyword:
                    - "supports" if they reinforce each other
                    - "contrasts" if they present different views
                    - "extends" if one builds on the other
                    - "questions" if one challenges the other
                    - "none" if there's no strong relationship

                    Return ONLY the keyword.
                    """;

                var relationship = await _semanticKernelService.CompleteTextAsync(relationshipPrompt);
                
                if (relationship.Trim().ToLower() != "none")
                {
                    links.Add(new
                    {
                        source = source1.Id,
                        target = source2.Id,
                        type = relationship.Trim().ToLower()
                    });
                }
            }
        }

        // 4. Generate synthesis points (key insights at intersection points)
        var synthesisPoints = new List<object>();
        foreach (var theme in themes.Themes)
        {
            if (theme.SourceIndices.Count > 1)
            {
                var relatedSources = theme.SourceIndices
                    .Where(i => i <= sources.Length)
                    .Select(i => sources[i - 1]);

                var synthesisPrompt = $"""
                    These sources share the theme "{theme.Label}":
                    {string.Join("\n\n", relatedSources.Select(s => s.Content))}

                    Identify a key synthesis point (insight that emerges from combining these sources).
                    Respond with a clear, specific insight in 1-2 sentences.
                    """;

                var insight = await _semanticKernelService.CompleteTextAsync(synthesisPrompt);
                
                var pointId = $"point_{Guid.NewGuid()}";
                nodes.Add(new
                {
                    id = pointId,
                    label = insight,
                    type = "synthesis"
                });

                // Connect synthesis point to relevant sources
                foreach (var source in relatedSources)
                {
                    links.Add(new
                    {
                        source = source.Id,
                        target = pointId,
                        type = "synthesis"
                    });
                }
            }
        }

        // Return the complete synthesis map
        var synthesisMap = new
        {
            nodes,
            links,
            clusters,
            metadata = new
            {
                sourceCount = sources.Length,
                themeCount = themes.Themes.Count,
                synthesisPointCount = synthesisPoints.Count
            }
        };

        return System.Text.Json.JsonSerializer.Serialize(synthesisMap);
    }

    private class ThemesResponse
    {
        public List<Theme> Themes { get; set; }
    }

    private class Theme
    {
        public string Label { get; set; }
        public List<int> SourceIndices { get; set; }
        public string Description { get; set; }
    }

    public async Task<IEnumerable<RemixNoteDto>> GetSuggestionsAsync(Guid remixId)
    {
        var notes = await _noteRepository.GetUnusedSuggestionsAsync(remixId);
        return notes.Select(MapRemixNote);
    }

    public async Task<RemixScoreResult> CalculateRemixScoreAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        var insights = new List<ScoreInsight>();
        var sources = await Task.WhenAll(remix.Sources.Select(async s => 
        {
            var post = await _postRepository.GetByIdAsync(s.SourcePostId);
            return new { Post = post, Source = s };
        }));

        // 1. Synthesis Score - How well does it combine and build upon sources?
        var synthesisPrompt = $"""
            Evaluate how well this remix synthesizes its source materials:

            Remix Content:
            {remix.Content}

            Source Materials:
            {string.Join("\n\n", sources.Select(s => $"Source ({s.Source.Relationship}): {s.Post.Content}"))}

            Score these aspects from 0.0 to 1.0:
            1. Integration - How well are sources woven together?
            2. Development - Does it build new insights beyond the sources?
            3. Balance - Are sources used proportionally and appropriately?

            Format response as JSON:
            {{
                "integration": 0.0,
                "development": 0.0,
                "balance": 0.0,
                "explanation": "Brief explanation of scores"
            }}
            """;

        var synthesisResult = await _semanticKernelService.CompleteTextAsync(synthesisPrompt);
        var synthesisScores = System.Text.Json.JsonSerializer.Deserialize<SynthesisScores>(synthesisResult);
        var synthesisScore = (synthesisScores.Integration + synthesisScores.Development + synthesisScores.Balance) / 3;
        
        insights.Add(new ScoreInsight 
        { 
            Category = "Synthesis",
            Score = synthesisScore,
            Explanation = synthesisScores.Explanation
        });

        // 2. Cohesion Score - How well-structured and logically connected is it?
        var cohesionPrompt = $"""
            Evaluate the cohesion and structure of this remix:

            Content:
            {remix.Content}

            Score these aspects from 0.0 to 1.0:
            1. Flow - How smoothly do ideas connect?
            2. Structure - How clear is the organization?
            3. Clarity - How well are ideas expressed?

            Format response as JSON:
            {{
                "flow": 0.0,
                "structure": 0.0,
                "clarity": 0.0,
                "explanation": "Brief explanation of scores"
            }}
            """;

        var cohesionResult = await _semanticKernelService.CompleteTextAsync(cohesionPrompt);
        var cohesionScores = System.Text.Json.JsonSerializer.Deserialize<CohesionScores>(cohesionResult);
        var cohesionScore = (cohesionScores.Flow + cohesionScores.Structure + cohesionScores.Clarity) / 3;

        insights.Add(new ScoreInsight 
        { 
            Category = "Cohesion",
            Score = cohesionScore,
            Explanation = cohesionScores.Explanation
        });

        // 3. Novelty Score - How original and innovative is the synthesis?
        var originalityPrompt = $"""
            Evaluate the originality of this remix compared to its sources:

            Remix:
            {remix.Content}

            Sources:
            {string.Join("\n\n", sources.Select(s => s.Post.Content))}

            Score these aspects from 0.0 to 1.0:
            1. Innovation - Does it present new ideas or perspectives?
            2. Transformation - How much does it transform source material?
            3. Insight - Does it provide unique insights?

            Format response as JSON:
            {{
                "innovation": 0.0,
                "transformation": 0.0,
                "insight": 0.0,
                "explanation": "Brief explanation of scores"
            }}
            """;

        var noveltyResult = await _semanticKernelService.CompleteTextAsync(originalityPrompt);
        var noveltyScores = System.Text.Json.JsonSerializer.Deserialize<NoveltyScores>(noveltyResult);
        var noveltyScore = (noveltyScores.Innovation + noveltyScores.Transformation + noveltyScores.Insight) / 3;

        insights.Add(new ScoreInsight 
        { 
            Category = "Novelty",
            Score = noveltyScore,
            Explanation = noveltyScores.Explanation
        });

        // 4. Source Utilization Score - How effectively are sources used?
        var sourceUtilization = CalculateSourceUtilization(remix, sources.Select(s => s.Post).ToList());
        insights.Add(new ScoreInsight 
        { 
            Category = "Source Utilization",
            Score = sourceUtilization.Score,
            Explanation = sourceUtilization.Explanation
        });

        // Calculate final score with weighted components
        var finalScore = new[]
        {
            synthesisScore * 0.35m,      // Synthesis is most important
            cohesionScore * 0.25m,       // Cohesion is second
            noveltyScore * 0.20m,        // Novelty is third
            sourceUtilization.Score * 0.20m  // Source utilization is fourth
        }.Sum();

        return new RemixScoreResult
        {
            FinalScore = finalScore,
            SynthesisScore = synthesisScore,
            CohesionScore = cohesionScore,
            NoveltyScore = noveltyScore,
            SourceUtilizationScore = sourceUtilization.Score,
            Insights = insights
        };
    }

    private (decimal Score, string Explanation) CalculateSourceUtilization(Remix remix, List<Post> sources)
    {
        // Start with perfect score and deduct based on issues
        decimal score = 1.0m;
        var issues = new List<string>();

        // Check source count
        if (sources.Count < 2)
        {
            score -= 0.3m;
            issues.Add("Too few sources for effective synthesis");
        }
        else if (sources.Count > 10)
        {
            score -= 0.2m;
            issues.Add("Large number of sources may dilute focus");
        }

        // Check content length relative to sources
        var sourceWords = sources.Sum(s => s.Content.Split().Length);
        var remixWords = remix.Content.Split().Length;
        var ratio = (decimal)remixWords / sourceWords;

        if (ratio < 0.3m)
        {
            score -= 0.2m;
            issues.Add("Remix may be too brief relative to source material");
        }
        else if (ratio > 2.0m)
        {
            score -= 0.1m;
            issues.Add("Remix may be overly verbose");
        }

        // Check relationship distribution
        var relationships = remix.Sources
            .GroupBy(s => s.Relationship)
            .ToDictionary(g => g.Key, g => g.Count());

        if (relationships.Count == 1)
        {
            score -= 0.2m;
            issues.Add("Limited variety in source relationships");
        }

        // Check quote usage (if available)
        var quotedExcerpts = remix.Sources.SelectMany(s => s.QuotedExcerpts).Count();
        if (quotedExcerpts == 0)
        {
            score -= 0.1m;
            issues.Add("No direct quotations from sources");
        }
        else if (quotedExcerpts > sources.Count * 3)
        {
            score -= 0.1m;
            issues.Add("Heavy reliance on direct quotations");
        }

        // Ensure score stays in valid range
        score = Math.Max(0.0m, Math.Min(1.0m, score));

        return (score, string.Join(". ", issues));
    }

    private class SynthesisScores
    {
        public decimal Integration { get; set; }
        public decimal Development { get; set; }
        public decimal Balance { get; set; }
        public string Explanation { get; set; }
    }

    private class CohesionScores
    {
        public decimal Flow { get; set; }
        public decimal Structure { get; set; }
        public decimal Clarity { get; set; }
        public string Explanation { get; set; }
    }

    private class NoveltyScores
    {
        public decimal Innovation { get; set; }
        public decimal Transformation { get; set; }
        public decimal Insight { get; set; }
        public string Explanation { get; set; }
    }
    }

    private static RemixSourceDto MapRemixSource(RemixSource source)
    {
        return new RemixSourceDto
        {
            SourcePostId = source.SourcePostId,
            PostTitle = source.SourcePost.Title,
            AuthorUsername = source.SourcePost.Author.Username,
            Relationship = source.Relationship,
            Context = source.Context,
            Order = source.Order,
            Quotes = source.Quotes.Select(q => new QuoteDto
            {
                Text = q.Text,
                StartPosition = q.StartPosition,
                EndPosition = q.EndPosition,
                Context = q.Context
            }).ToList(),
            RelevanceScores = source.RelevanceScores
        };
    }

    private static RemixNoteDto MapRemixNote(RemixNote note)
    {
        return new RemixNoteDto
        {
            Id = note.Id,
            Type = note.Type,
            Content = note.Content,
            RelatedSourceIds = note.RelatedSourceIds,
            Confidence = note.Confidence,
            IsApplied = note.IsApplied
        };
    }
}