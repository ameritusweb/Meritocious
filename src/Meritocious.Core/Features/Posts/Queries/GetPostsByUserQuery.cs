using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Posts.Queries
{
    public record GetPostsByUserQuery : IRequest<Result<List<PostDto>>>
    {
        public Guid UserId { get; init; }
        public string SortBy { get; init; } = "date"; // date, merit
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}
