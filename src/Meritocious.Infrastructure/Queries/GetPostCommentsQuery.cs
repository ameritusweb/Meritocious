using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Queries
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;
    using Meritocious.Infrastructure.Data.Repositories;

    public record GetPostCommentsQuery : IRequest<Result<List<Comment>>>
    {
        public Guid PostId { get; init; }
        public string SortBy { get; init; } = "merit"; // merit, date, thread
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}