using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Search.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Content;
    using Meritocious.Common.Enums;

    public record SearchContentQuery : IRequest<SearchResultDto>
    {
        public string SearchTerm { get; init; }
        public List<ContentType> ContentTypes { get; init; } = new();
        public string SortBy { get; init; } = "relevance"; // relevance, merit, date
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public Dictionary<string, string> Filters { get; init; } = new();
    }

    public class SearchResultDto
    {
        public List<SearchItemDto> Items { get; set; } = new();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<FacetDto> Facets { get; set; } = new();
    }

    public class SearchItemDto
    {
        public string Id { get; set; }
        public ContentType Type { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> HighlightedTerms { get; set; } = new();
    }

    public class FacetDto
    {
        public string Name { get; set; }
        public Dictionary<string, int> Values { get; set; } = new();
    }
}