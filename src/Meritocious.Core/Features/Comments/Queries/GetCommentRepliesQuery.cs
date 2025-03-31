using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Queries
{
    public record GetCommentRepliesQuery : IRequest<Result<List<CommentDto>>>
    {
        public Guid CommentId { get; init; }
        public string SortBy { get; init; } = "merit"; // merit, date
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}
