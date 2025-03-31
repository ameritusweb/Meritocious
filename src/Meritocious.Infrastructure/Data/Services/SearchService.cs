using Meritocious.Common.Enums;
using Meritocious.Core.Features.Search.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Services
{
    public class SearchService : ISearchService
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<SearchService> _logger;

        public SearchService(
            MeritociousDbContext context,
            ILogger<SearchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SearchResultDto> SearchAsync(
            string searchTerm,
            List<ContentType> contentTypes,
            Dictionary<string, string> filters,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Normalize search term
                searchTerm = searchTerm?.Trim().ToLowerInvariant() ?? "";

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new SearchResultDto
                    {
                        Items = new List<SearchItemDto>(),
                        TotalResults = 0,
                        CurrentPage = page,
                        TotalPages = 0,
                        Facets = new List<FacetDto>()
                    };
                }

                // If no content types specified, search all types
                if (contentTypes == null || !contentTypes.Any())
                {
                    contentTypes = new List<ContentType>
                    {
                        ContentType.Post,
                        ContentType.Comment
                    };
                }

                // Initialize results
                var results = new List<SearchItemDto>();
                var facets = new Dictionary<string, Dictionary<string, int>>();

                // Search posts
                if (contentTypes.Contains(ContentType.Post))
                {
                    var postResults = await SearchPostsAsync(searchTerm, filters);
                    results.AddRange(postResults);

                    // Add facets for post tags
                    if (!facets.ContainsKey("tags"))
                    {
                        facets["tags"] = new Dictionary<string, int>();
                    }

                    foreach (var post in postResults)
                    {
                        // In a real implementation, we would fetch and count tags
                        // This is a placeholder
                        var tags = await _context.Posts
                            .Where(p => p.Id == post.Id)
                            .SelectMany(p => p.Tags.Select(t => t.Name))
                            .ToListAsync();

                        foreach (var tag in tags)
                        {
                            if (!facets["tags"].ContainsKey(tag))
                            {
                                facets["tags"][tag] = 0;
                            }
                            facets["tags"][tag]++;
                        }
                    }
                }

                // Search comments
                if (contentTypes.Contains(ContentType.Comment))
                {
                    var commentResults = await SearchCommentsAsync(searchTerm, filters);
                    results.AddRange(commentResults);
                }

                // Apply sorting (in a real implementation, this would be smarter)
                results = results.OrderByDescending(r => r.MeritScore).ToList();

                // Apply pagination
                var totalResults = results.Count;
                var totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
                var paginatedResults = results
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Convert facets to DTO
                var facetDtos = facets.Select(f => new FacetDto
                {
                    Name = f.Key,
                    Values = f.Value
                }).ToList();

                return new SearchResultDto
                {
                    Items = paginatedResults,
                    TotalResults = totalResults,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    Facets = facetDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        private async Task<List<SearchItemDto>> SearchPostsAsync(
            string searchTerm,
            Dictionary<string, string> filters)
        {
            var query = _context.Posts
                .Where(p => !p.IsDeleted)
                .Where(p => p.Title.ToLower().Contains(searchTerm) ||
                           p.Content.ToLower().Contains(searchTerm));

            // Apply filters
            if (filters != null)
            {
                if (filters.TryGetValue("authorId", out var authorId))
                {
                    if (Guid.TryParse(authorId, out var authorGuid))
                    {
                        query = query.Where(p => p.AuthorId == authorGuid);
                    }
                }

                if (filters.TryGetValue("minMeritScore", out var minScoreStr))
                {
                    if (decimal.TryParse(minScoreStr, out var minScore))
                    {
                        query = query.Where(p => p.MeritScore >= minScore);
                    }
                }

                if (filters.TryGetValue("tag", out var tag))
                {
                    query = query.Where(p => p.Tags.Any(t => t.Name == tag));
                }
            }

            var posts = await query
                .Include(p => p.Author)
                .ToListAsync();

            return posts.Select(p => new SearchItemDto
            {
                Id = p.Id,
                Type = ContentType.Post,
                Title = p.Title,
                Excerpt = GetExcerpt(p.Content, searchTerm),
                AuthorId = p.AuthorId,
                AuthorUsername = p.Author.Username,
                MeritScore = p.MeritScore,
                CreatedAt = p.CreatedAt,
                HighlightedTerms = ExtractHighlightedTerms(p.Content, searchTerm)
            }).ToList();
        }

        private async Task<List<SearchItemDto>> SearchCommentsAsync(
            string searchTerm,
            Dictionary<string, string> filters)
        {
            var query = _context.Comments
                .Where(c => !c.IsDeleted)
                .Where(c => c.Content.ToLower().Contains(searchTerm));

            // Apply filters
            if (filters != null)
            {
                if (filters.TryGetValue("authorId", out var authorId))
                {
                    if (Guid.TryParse(authorId, out var authorGuid))
                    {
                        query = query.Where(c => c.AuthorId == authorGuid);
                    }
                }

                if (filters.TryGetValue("minMeritScore", out var minScoreStr))
                {
                    if (decimal.TryParse(minScoreStr, out var minScore))
                    {
                        query = query.Where(c => c.MeritScore >= minScore);
                    }
                }

                if (filters.TryGetValue("postId", out var postId))
                {
                    if (Guid.TryParse(postId, out var postGuid))
                    {
                        query = query.Where(c => c.PostId == postGuid);
                    }
                }
            }

            var comments = await query
                .Include(c => c.Author)
                .Include(c => c.Post)
                .ToListAsync();

            return comments.Select(c => new SearchItemDto
            {
                Id = c.Id,
                Type = ContentType.Comment,
                Title = $"Comment on {c.Post.Title}",
                Excerpt = GetExcerpt(c.Content, searchTerm),
                AuthorId = c.AuthorId,
                AuthorUsername = c.Author.Username,
                MeritScore = c.MeritScore,
                CreatedAt = c.CreatedAt,
                HighlightedTerms = ExtractHighlightedTerms(c.Content, searchTerm)
            }).ToList();
        }

        private string GetExcerpt(string content, string searchTerm, int length = 200)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            // Find the position of the search term
            var index = content.ToLower().IndexOf(searchTerm.ToLower());
            if (index == -1)
            {
                // If term not found, return first part of content
                return content.Length <= length
                    ? content
                    : content.Substring(0, length) + "...";
            }

            // Calculate start and end positions for excerpt
            var start = Math.Max(0, index - length / 2);
            var end = Math.Min(content.Length, index + searchTerm.Length + length / 2);

            var excerpt = content.Substring(start, end - start);

            // Add ellipsis if we're not at the bounds
            if (start > 0) excerpt = "..." + excerpt;
            if (end < content.Length) excerpt = excerpt + "...";

            return excerpt;
        }

        private List<string> ExtractHighlightedTerms(string content, string searchTerm)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(searchTerm))
                return new List<string>();

            // Extract words that contain the search term
            var pattern = $@"\b\w*{Regex.Escape(searchTerm)}\w*\b";
            var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            return matches.Select(m => m.Value).Distinct().ToList();
        }
    }
}
