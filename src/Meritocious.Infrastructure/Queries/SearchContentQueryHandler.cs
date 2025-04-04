using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Search.Queries
{
    using MediatR;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.Core.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Meritocious.Common.Enums;

    public class SearchContentQueryHandler : IRequestHandler<SearchContentQuery, SearchResultDto>
    {
        private readonly PostRepository postRepository;
        private readonly CommentRepository commentRepository;
        private readonly ISearchService searchService;
        private readonly ILogger<SearchContentQueryHandler> logger;

        public SearchContentQueryHandler(
            PostRepository postRepository,
            CommentRepository commentRepository,
            ISearchService searchService,
            ILogger<SearchContentQueryHandler> logger)
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
            this.searchService = searchService;
            this.logger = logger;
        }

        public async Task<SearchResultDto> Handle(SearchContentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Perform search using the search service
                var searchResults = await searchService.SearchAsync(
                    request.SearchTerm,
                    request.ContentTypes,
                    request.Filters,
                    request.Page,
                    request.PageSize);

                // Get detailed content for each result
                var items = new List<SearchItemDto>();
                foreach (var result in searchResults.Items)
                {
                    switch (result.Type)
                    {
                        case ContentType.Post:
                            var post = await postRepository.GetByIdAsync(result.Id);
                            if (post != null)
                            {
                                items.Add(new SearchItemDto
                                {
                                    Id = post.Id,
                                    Type = ContentType.Post,
                                    Title = post.Title,
                                    Excerpt = GetExcerpt(post.Content, request.SearchTerm),
                                    AuthorId = post.AuthorId,
                                    AuthorUsername = post.Author.UserName,
                                    MeritScore = post.MeritScore,
                                    CreatedAt = post.CreatedAt,
                                    HighlightedTerms = result.HighlightedTerms
                                });
                            }
                            break;

                        case ContentType.Comment:
                            var comment = await commentRepository.GetByIdAsync(result.Id);
                            if (comment != null)
                            {
                                items.Add(new SearchItemDto
                                {
                                    Id = comment.Id,
                                    Type = ContentType.Comment,
                                    Title = $"Comment on {comment.Post.Title}",
                                    Excerpt = GetExcerpt(comment.Content, request.SearchTerm),
                                    AuthorId = comment.AuthorId,
                                    AuthorUsername = comment.Author.UserName,
                                    MeritScore = comment.MeritScore,
                                    CreatedAt = comment.CreatedAt,
                                    HighlightedTerms = result.HighlightedTerms
                                });
                            }
                            break;
                    }
                }

                // Sort results if needed
                items = request.SortBy switch
                {
                    "merit" => items.OrderByDescending(i => i.MeritScore).ToList(),
                    "date" => items.OrderByDescending(i => i.CreatedAt).ToList(),
                    _ => items // Keep original relevance-based ordering
                };

                return new SearchResultDto
                {
                    Items = items,
                    TotalResults = searchResults.TotalResults,
                    CurrentPage = request.Page,
                    TotalPages = (int)Math.Ceiling(searchResults.TotalResults / (double)request.PageSize),
                    Facets = searchResults.Facets
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error performing content search for term: {SearchTerm}", request.SearchTerm);
                throw;
            }
        }

        private string GetExcerpt(string content, string searchTerm, int length = 200)
        {
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
            if (start > 0)
            {
                excerpt = "..." + excerpt;
            }

            if (end < content.Length)
            {
                excerpt = excerpt + "...";
            }

            return excerpt;
        }
    }
}