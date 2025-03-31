using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Queries
{
    public record GetPostsByUserQuery : IRequest<Result<List<Post>>>
    {
        public Guid UserId { get; init; }
        public string SortBy { get; init; } = "date"; // date, merit
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}
