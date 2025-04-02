using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;

namespace Meritocious.Infrastructure.Data.Services;

public class RemixService : IRemixService
{
    private readonly IRemixRepository _remixRepository;
    private readonly IRemixSourceRepository _sourceRepository;
    private readonly IRemixNoteRepository _noteRepository;
    private readonly IPostRepository _postRepository;
    private readonly IMeritScoringService _meritScoringService;
    private readonly ISemanticSearchService _semanticSearchService;
    private readonly ITagService _tagService;

    public RemixService(
        IRemixRepository remixRepository,
        IRemixSourceRepository sourceRepository,
        IRemixNoteRepository noteRepository,
        IPostRepository postRepository,
        IMeritScoringService meritScoringService,
        ISemanticSearchService semanticSearchService,
        ITagService tagService)
    {
        _remixRepository = remixRepository;
        _sourceRepository = sourceRepository;
        _noteRepository = noteRepository;
        _postRepository = postRepository;
        _meritScoringService = meritScoringService;
        _semanticSearchService = semanticSearchService;
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
        var source = await _sourceRepository.GetByIdAsync(sourceId);
        if (source == null) throw new ArgumentException("Source not found");

        var quotes = source.QuotedExcerpts.ToList();
        quotes.Add(request.Text);
        await _sourceRepository.UpdateQuotesAsync(sourceId, quotes);
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
                // TODO: Implement engagement metrics
                Views = 0,
                Likes = 0,
                Comments = 0,
                Forks = 0,
                SourceInfluenceScores = remix.Sources.ToDictionary(
                    s => s.SourcePost.Title,
                    s => s.RelevanceScores.Values.DefaultIfEmpty(0).Average()
                )
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

        // TODO: Implement AI insight generation
        var notes = new List<RemixNote>();
        
        await _noteRepository.AddRangeAsync(notes);
        return notes.Select(MapRemixNote);
    }

    public async Task<string> GenerateSynthesisMapAsync(Guid remixId)
    {
        var remix = await _remixRepository.GetByIdWithFullDetailsAsync(remixId);
        if (remix == null) throw new ArgumentException("Remix not found");

        // TODO: Implement AI synthesis map generation
        return "{}";
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

        // TODO: Implement comprehensive scoring
        return new RemixScoreResult
        {
            FinalScore = 0.0m,
            SynthesisScore = 0.0m,
            CohesionScore = 0.0m,
            NoveltyScore = 0.0m,
            SourceUtilizationScore = 0.0m,
            Insights = new List<ScoreInsight>()
        };
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
            Quotes = source.QuotedExcerpts.Select(q => new QuoteDto
            {
                Text = q,
                StartPosition = 0, // TODO: Store position info
                EndPosition = 0,
                Context = ""
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