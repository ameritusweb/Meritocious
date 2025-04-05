using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public record GetPostsByTagQuery : IRequest<Result<List<PostDto>>>
    {
        public string TagId { get; init; }
        public string SortBy { get; init; } = "merit";
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}
