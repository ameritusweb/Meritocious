using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;
using Meritocious.AI.SemanticKernel.Interfaces;
using Meritocious.Infrastructure.Data.Repositories;
using Meritocious.AI.MeritScoring.Interfaces;
using Meritocious.Core.Events;

namespace Meritocious.Infrastructure.Data.Services;

public interface IRemixService
{
    Task<RemixDto> CreateRemixAsync(CreateRemixRequest request);
    Task<RemixDto> UpdateRemixAsync(Guid remixId, UpdateRemixRequest request);
    Task<RemixDto> GetRemixByIdAsync(Guid remixId);
    Task<bool> DeleteRemixAsync(Guid remixId, Guid userId);
    Task<RemixDto> PublishRemixAsync(Guid remixId, Guid userId);
    Task<RemixSourceDto> AddSourceAsync(Guid remixId, AddSourceRequest request);
    Task<bool> RemoveSourceAsync(Guid remixId, Guid sourceId);
    Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<SourceOrderUpdate> updates);
    Task<RemixSourceDto> UpdateSourceRelationshipAsync(Guid remixId, Guid sourceId, string relationship);
    Task AddQuoteToSourceAsync(Guid sourceId, AddQuoteRequest request);
    Task<IEnumerable<RemixDto>> GetUserRemixesAsync(Guid userId, RemixFilter filter);
    Task<IEnumerable<RemixDto>> GetRelatedRemixesAsync(Guid remixId, int limit = 5);
    Task<IEnumerable<RemixDto>> GetTrendingRemixesAsync(int limit = 10);
    Task RecordEngagementAsync(Guid remixId, RemixEngagementEvent engagementEvent);
    Task<RemixAnalytics> GetRemixAnalyticsAsync(Guid remixId);
    Task<IEnumerable<RemixDto>> SearchRemixesAsync(RemixSearchRequest request);
    Task<IEnumerable<RemixNoteDto>> GenerateInsightsAsync(Guid remixId);
    Task<string> GenerateSynthesisMapAsync(Guid remixId);
    Task<IEnumerable<RemixNoteDto>> GetSuggestionsAsync(Guid remixId);
    Task<RemixScoreResult> CalculateRemixScoreAsync(Guid remixId);
}

/// <summary>
/// Service for handling remixes (posts that synthesize content from multiple source posts).
/// A remix is a special type of post that has relationships with its source posts through PostRelation entities.
/// </summary>
public class RemixService : IRemixService
{
    private readonly IPostRepository _postRepository;
    private readonly IMeritScoringService _meritScoringService;
    private readonly ISemanticSearchService _semanticSearchService;
    private readonly ISemanticKernelService _semanticKernelService;
    private readonly ITagService _tagService;

    public RemixService(
        IPostRepository postRepository,
        IMeritScoringService meritScoringService,
        ISemanticSearchService semanticSearchService,
        ISemanticKernelService semanticKernelService,
        ITagService tagService)
    {
        _postRepository = postRepository;
        _meritScoringService = meritScoringService;
        _semanticSearchService = semanticSearchService;
        _semanticKernelService = semanticKernelService;
        _tagService = tagService;
    }

    public async Task<RemixDto> CreateRemixAsync(CreateRemixRequest request)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.InitialContent,
            AuthorId = request.AuthorId,
            SubstackId = request.SubstackId,
            IsDraft = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Type = "remix"
        };

        // Add tags
        if (request.Tags.Any())
        {
            post.Tags = await _tagService.GetOrCreateTagsAsync(request.Tags);
        }

        // Add post to repository first to get its ID
        await _postRepository.AddAsync(post);

        // Add initial source relationships
        if (request.InitialSourceIds.Any())
        {
            var sourceOrder = 0;
            foreach (var sourceId in request.InitialSourceIds)
            {
                var relation = PostRelation.CreateRemixSource(
                    await _postRepository.GetByIdAsync(sourceId), 
                    post, 
                    "support", // Default relationship
                    sourceOrder++);
                
                await _postRepository.AddSourceAsync(post.Id, relation);
            }
        }

        // Generate initial synthesis map and notes
        if (request.InitialSourceIds.Any())
        {
            var synthesisMap = await GenerateSynthesisMapAsync(post.Id);
            await _postRepository.UpdateSynthesisMapAsync(post.Id, synthesisMap);
            await GenerateInsightsAsync(post.Id);
        }

        return await GetRemixByIdAsync(post.Id);
    }

    public async Task<RemixDto> UpdateRemixAsync(Guid remixId, UpdateRemixRequest request)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix") 
            throw new ArgumentException("Remix post not found");

        // Update basic properties
        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;

        // Update tags
        if (request.Tags != null)
        {
            post.Tags = await _tagService.GetOrCreateTagsAsync(request.Tags);
        }

        // Update synthesis map if requested
        if (request.UpdateSynthesisMap)
        {
            var synthesisMap = await GenerateSynthesisMapAsync(remixId);
            await _postRepository.UpdateSynthesisMapAsync(remixId, synthesisMap);
        }

        // Save changes
        await _postRepository.UpdateAsync(post);

        return await GetRemixByIdAsync(remixId);
    }

    public async Task<RemixDto> GetRemixByIdAsync(Guid remixId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
        {
            throw new ArgumentException("Remix post not found");
        }

        // Get parent relations (remix sources)
        var sources = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .OrderBy(r => r.OrderIndex)
            .Select(r => new RemixSourceDto
            {
                SourcePostId = r.ParentId,
                PostTitle = r.Parent.Title,
                AuthorUsername = r.Parent.Author.Username,
                Relationship = r.Role,
                Context = r.Context,
                Order = r.OrderIndex,
                Quotes = r.Quotes.Select(q => new QuoteDto
                {
                    Text = q.Content,
                    StartPosition = q.StartPosition,
                    EndPosition = q.EndPosition,
                    Context = q.Context
                }).ToList()
            })
            .ToList();

        // Map notes
        var notes = post.Notes.Select(n => new RemixNoteDto
        {
            Id = n.Id,
            Type = n.Type,
            Content = n.Content,
            RelatedSourceIds = n.RelatedSourceIds,
            Confidence = n.Confidence,
            IsApplied = n.IsApplied
        }).ToList();

        return new RemixDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorUsername = post.Author.Username,
            AuthorId = post.AuthorId,
            MeritScore = post.MeritScore,
            Sources = sources,
            Tags = post.Tags.Select(t => t.Name).ToList(),
            SubstackId = post.SubstackId,
            IsDraft = post.IsDraft,
            PublishedAt = post.PublishedAt,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Notes = notes
        };
    }

    public async Task<bool> DeleteRemixAsync(Guid remixId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(remixId);
        if (post == null || post.Type != "remix" || post.AuthorId != userId) 
            return false;

        await _postRepository.DeleteAsync(post);
        return true;
    }

    public async Task<RemixDto> PublishRemixAsync(Guid remixId, Guid userId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix" || post.AuthorId != userId) 
            throw new ArgumentException("Remix post not found or unauthorized");

        // Calculate final merit score
        var scoreResult = await CalculateRemixScoreAsync(remixId);
        post.MeritScore = scoreResult.FinalScore;

        // Update status
        post.IsDraft = false;
        post.PublishedAt = DateTime.UtcNow;
        
        await _postRepository.UpdateAsync(post);
        return await GetRemixByIdAsync(remixId);
    }

    public async Task<RemixSourceDto> AddSourceAsync(Guid remixId, AddSourceRequest request)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
            throw new ArgumentException("Remix post not found");

        var sourcePost = await _postRepository.GetByIdAsync(request.PostId);
        if (sourcePost == null)
            throw new ArgumentException("Source post not found");

        // Create relation
        var sourceOrder = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .Count();

        var relation = PostRelation.CreateRemixSource(
            sourcePost,
            post,
            request.Relationship,
            sourceOrder,
            request.Context);

        await _postRepository.AddSourceAsync(remixId, relation);

        // Add initial quotes if provided
        if (request.InitialQuotes?.Any() == true)
        {
            await _postRepository.UpdateQuotesAsync(relation.Id, request.InitialQuotes);
        }

        // Update synthesis map and generate new insights
        var synthesisMap = await GenerateSynthesisMapAsync(remixId);
        await _postRepository.UpdateSynthesisMapAsync(remixId, synthesisMap);
        await GenerateInsightsAsync(remixId);

        // Return the newly created source
        var sources = post.ParentRelations
            .Where(r => r.RelationType == "remix" && r.ParentId == request.PostId)
            .Select(r => new RemixSourceDto
            {
                SourcePostId = r.ParentId,
                PostTitle = sourcePost.Title,
                AuthorUsername = sourcePost.Author.Username,
                Relationship = r.Role,
                Context = r.Context,
                Order = r.OrderIndex,
                Quotes = r.Quotes.Select(q => new QuoteDto
                {
                    Text = q.Content,
                    StartPosition = q.StartPosition,
                    EndPosition = q.EndPosition,
                    Context = q.Context
                }).ToList()
            })
            .First();

        return sources;
    }

    public async Task<bool> RemoveSourceAsync(Guid remixId, Guid sourceId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix") 
            return false;

        var relation = post.ParentRelations
            .FirstOrDefault(r => r.RelationType == "remix" && r.ParentId == sourceId);
        if (relation == null) 
            return false;

        await _postRepository.RemoveSourceAsync(remixId, sourceId);

        // Update synthesis map
        var synthesisMap = await GenerateSynthesisMapAsync(remixId);
        await _postRepository.UpdateSynthesisMapAsync(remixId, synthesisMap);

        return true;
    }

    public async Task UpdateSourceOrderAsync(Guid remixId, IEnumerable<SourceOrderUpdate> updates)
    {
        var orderUpdates = updates.Select(u => (u.SourceId, u.NewOrder));
        await _postRepository.UpdateSourceOrderAsync(remixId, orderUpdates);
    }

    public async Task<RemixSourceDto> UpdateSourceRelationshipAsync(Guid remixId, Guid sourceId, string relationship)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
        {
            throw new ArgumentException("Remix post not found");
        }

        var relation = post.ParentRelations
            .FirstOrDefault(r => r.RelationType == "remix" && r.ParentId == sourceId);
        if (relation == null)
        {
            throw new ArgumentException("Source relation not found");
        }

        // Update the relationship role
        relation.UpdateRole(relationship);
        await _postRepository.UpdateAsync(post);

        // Update synthesis map
        var synthesisMap = await GenerateSynthesisMapAsync(remixId);
        await _postRepository.UpdateSynthesisMapAsync(remixId, synthesisMap);

        return new RemixSourceDto
        {
            SourcePostId = relation.ParentId,
            PostTitle = relation.Parent.Title,
            AuthorUsername = relation.Parent.Author.Username,
            Relationship = relation.Role,
            Context = relation.Context,
            Order = relation.OrderIndex,
            Quotes = relation.Quotes.Select(q => new QuoteDto
            {
                Text = q.Content,
                StartPosition = q.StartPosition,
                EndPosition = q.EndPosition,
                Context = q.Context
            }).ToList()
        };
    }

    public async Task AddQuoteToSourceAsync(Guid sourceId, AddQuoteRequest request)
    {
        var relation = await _postRepository.GetByIdWithRelations(sourceId)
            .FirstOrDefaultAsync(r => r.RelationType == "remix");
        if (relation == null) 
            throw new ArgumentException("Source relation not found");

        var sourcePost = await _postRepository.GetByIdAsync(relation.ParentId);
        if (sourcePost == null)
        {
            throw new ArgumentException("Source post not found");
        }

        // Find the quote in the source content
        var startIndex = sourcePost.Content.IndexOf(request.Text);
        if (startIndex == -1)
        {
            throw new ArgumentException("Quote not found in source content");
        }

        // Get some context around the quote
        var contextStart = Math.Max(0, startIndex - 100);
        var contextLength = Math.Min(startIndex + request.Text.Length + 100, sourcePost.Content.Length) - contextStart;
        var context = sourcePost.Content.Substring(contextStart, contextLength);

        // Create quote location
        var quote = new QuoteLocation
        {
            StartPosition = startIndex,
            EndPosition = startIndex + request.Text.Length,
            Content = request.Text,
            Context = context
        };

        relation.AddQuote(quote);
        await _postRepository.UpdateAsync(relation);
    }

    public async Task<IEnumerable<RemixDto>> GetUserRemixesAsync(Guid userId, RemixFilter filter)
    {
        var posts = await _postRepository.GetUserRemixesAsync(userId, filter.IncludeDrafts);
        
        // Apply additional filters
        var filtered = posts.Where(p => 
            (!filter.FromDate.HasValue || p.CreatedAt >= filter.FromDate) &&
            (!filter.ToDate.HasValue || p.CreatedAt <= filter.ToDate) &&
            (!filter.Tags.Any() || p.Tags.Any(t => filter.Tags.Contains(t.Name))));

        // Apply sorting
        filtered = filter.SortBy switch
        {
            "merit" => filtered.OrderByDescending(p => p.MeritScore),
            "oldest" => filtered.OrderBy(p => p.CreatedAt),
            _ => filtered.OrderByDescending(p => p.CreatedAt) // "newest" is default
        };

        return filtered.Select(p => new RemixDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorUsername = p.Author.Username,
            MeritScore = p.MeritScore,
            Tags = p.Tags.Select(t => t.Name).ToList(),
            CreatedAt = p.CreatedAt,
            IsDraft = p.IsDraft
        });
    }

    public async Task<IEnumerable<RemixDto>> GetRelatedRemixesAsync(Guid remixId, int limit = 5)
    {
        var related = await _postRepository.GetRelatedRemixesAsync(remixId, limit);
        return related.Select(p => new RemixDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorUsername = p.Author.UserName,
            MeritScore = p.MeritScore,
            Tags = p.Tags.Select(t => t.Name).ToList()
        });
    }

    public async Task<IEnumerable<RemixDto>> GetTrendingRemixesAsync(int limit = 10)
    {
        var trending = await _postRepository.GetTrendingRemixesAsync(limit);
        return trending.Select(p => new RemixDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorUsername = p.Author.Username,
            MeritScore = p.MeritScore,
            Tags = p.Tags.Select(t => t.Name).ToList()
        });
    }

    public async Task RecordEngagementAsync(
        Guid remixId,
        RemixEngagementEvent engagementEvent)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
        {
            throw new ArgumentException("Remix post not found");
        }

        var engagement = await _postRepository.GetEngagementAsync(remixId);

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
            var relation = post.ParentRelations
                .FirstOrDefault(r => r.RelationType == "remix" && 
                                   r.ParentId == engagementEvent.SourceId.Value);
            
            if (relation != null)
            {
                relation.UpdateRelevanceScore(engagementEvent.InfluenceScore.Value);
            }
        }

        // Update sentiment if provided
        if (engagementEvent.SentimentScore.HasValue)
        {
            var oldScore = engagement.SentimentScore * (engagement.Comments - 1);
            engagement.SentimentScore = 
                (oldScore + engagementEvent.SentimentScore.Value) / engagement.Comments;
        }

        await _postRepository.UpdateAsync(post);
    }

    public async Task<RemixAnalytics> GetRemixAnalyticsAsync(Guid remixId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
        {
            throw new ArgumentException("Remix post not found");
        }

        var engagement = await _postRepository.GetEngagementAsync(remixId);
        var relations = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .ToList();

        return new RemixAnalytics
        {
            TotalSources = relations.Count,
            RelationshipDistribution = await _postRepository.GetRelationshipDistributionAsync(remixId),
            NoteTypeDistribution = await _postRepository.GetNoteTypeDistributionAsync(remixId),
            AverageSourceRelevance = relations.Average(r => r.RelevanceScore),
            TopTags = post.Tags.Select(t => t.Name).ToList(),
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
                SourceInfluenceScores = relations
                    .ToDictionary(
                        r => r.Parent.Title,
                        r => r.RelevanceScore
                    ),
                
                PeakEngagementTime = engagement.PeakEngagementTime,
                EngagementVelocity = engagement.EngagementVelocity,
                SentimentScore = engagement.SentimentScore,
                TopEngagementSources = relations
                    .OrderByDescending(r => r.RelevanceScore)
                    .Take(3)
                    .Select(r => r.Parent.Title)
                    .ToList()
            }
        };
    }

    public async Task<IEnumerable<RemixDto>> SearchRemixesAsync(RemixSearchRequest request)
    {
        // Use semantic search for better results
        var searchResults = await _semanticSearchService.SearchAsync(
            request.Query,
            entityType: "post",
            filters: new Dictionary<string, object>
            {
                { "type", "remix" },
                { "tags", request.Tags },
                { "minMeritScore", request.MinMeritScore }
            }
        );

        var postIds = searchResults.Select(r => Guid.Parse(r.Id)).ToList();
        var posts = await _postRepository.GetByIdsAsync(postIds);

        // Sort based on request
        var ordered = request.SortBy switch
        {
            "merit" => posts.OrderByDescending(p => p.MeritScore),
            "newest" => posts.OrderByDescending(p => p.CreatedAt),
            "oldest" => posts.OrderBy(p => p.CreatedAt),
            _ => posts.OrderBy(p => searchResults.FindIndex(sr => 
                sr.Id == p.Id.ToString())) // Maintain semantic search order
        };

        return ordered.Select(p => new RemixDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorUsername = p.Author.Username,
            MeritScore = p.MeritScore,
            Tags = p.Tags.Select(t => t.Name).ToList(),
            CreatedAt = p.CreatedAt
        });
    }

    public async Task<IEnumerable<RemixNoteDto>> GenerateInsightsAsync(Guid remixId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null)
            throw new ArgumentException("Post not found");

        // Get source contents through relations
        var sources = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .Select(r => new
            {
                Id = r.ParentId,
                Content = r.Parent.Content,
                Title = r.Parent.Title,
                Relationship = r.Role
            })
            .ToList();
        
        // 1. Generate connections between sources
        if (sources.Count > 1)
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
                        post.AddNote(
                            "Connection",
                            connection,
                            new List<Guid> { source1.Id, source2.Id },
                            confidence);
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
                post.AddNote(
                    "Insight",
                    insight,
                    new List<Guid> { source.Id },
                    confidence);
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

                Current post content:
                {post.Content}

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
                    post.AddNote(
                        "Suggestion",
                        suggestion.Trim(),
                        sources.Select(s => s.Id).ToList(),
                        confidence);
                }
            }
        }

        await _postRepository.UpdateAsync(post);

        return post.Notes.Select(n => new RemixNoteDto
        {
            Id = n.Id,
            Type = n.Type,
            Content = n.Content,
            RelatedSourceIds = n.RelatedSourceIds,
            Confidence = n.Confidence,
            IsApplied = n.IsApplied
        });
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
        if (wordCount < 10)
        {
            confidence -= 0.2m;
        }

        if (wordCount > 50)
        {
            confidence -= 0.1m;
        }

        // Penalize generic insights
        var genericPhrases = new[] { "it is important to", "one should consider", "it is interesting that" };
        confidence -= 0.15m * genericPhrases.Count(p => text.Contains(p));

        // Ensure confidence stays between 0 and 1
        return Math.Max(0.0m, Math.Min(1.0m, confidence));
    }

    public async Task<string> GenerateSynthesisMapAsync(Guid remixId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix")
        {
            throw new ArgumentException("Remix post not found");
        }

        var sourceRelations = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .ToList();

        var sources = await Task.WhenAll(sourceRelations.Select(async r => 
        {
            var sourcePost = await _postRepository.GetByIdAsync(r.ParentId);
            return new
            {
                Id = r.ParentId.ToString(),
                Title = sourcePost.Title,
                Content = sourcePost.Content,
                Relationship = r.Role,
                Order = r.OrderIndex
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
        var notes = await _postRepository.GetUnusedSuggestionsAsync(remixId);
        return notes.Select(n => new RemixNoteDto
        {
            Id = n.Id,
            Type = n.Type,
            Content = n.Content,
            RelatedSourceIds = n.RelatedSourceIds,
            Confidence = n.Confidence,
            IsApplied = n.IsApplied
        });
    }

    public async Task<RemixScoreResult> CalculateRemixScoreAsync(Guid remixId)
    {
        var post = await _postRepository.GetByIdWithFullDetailsAsync(remixId);
        if (post == null || post.Type != "remix") 
            throw new ArgumentException("Remix post not found");

        var insights = new List<ScoreInsight>();
        var sourceRelations = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .ToList();
            
        var sources = await Task.WhenAll(sourceRelations.Select(async r => 
        {
            var sourcePost = await _postRepository.GetByIdAsync(r.ParentId);
            return new { Post = sourcePost, Source = r };
        }));

        // 1. Synthesis Score - How well does it combine and build upon sources?
        var synthesisPrompt = $"""
            Evaluate how well this remix synthesizes its source materials:

            Post Content:
            {post.Content}

            Source Materials:
            {string.Join("\n\n", sources.Select(s => $"Source ({s.Source.Role}): {s.Post.Content}"))}

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
            Evaluate the cohesion and structure of this post:

            Content:
            {post.Content}

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
            Evaluate the originality of this post compared to its sources:

            Post:
            {post.Content}

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

    private (decimal Score, string Explanation) CalculateSourceUtilization(Post post, List<Post> sources)
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
        var remixWords = post.Content.Split().Length;
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
        var relationships = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .GroupBy(r => r.Role)
            .ToDictionary(g => g.Key, g => g.Count());

        if (relationships.Count == 1)
        {
            score -= 0.2m;
            issues.Add("Limited variety in source relationships");
        }

        // Check quote usage (if available)
        var quotedExcerpts = post.ParentRelations
            .Where(r => r.RelationType == "remix")
            .SelectMany(r => r.Quotes)
            .Count();

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

}