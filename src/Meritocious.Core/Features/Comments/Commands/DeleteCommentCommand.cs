using MediatR;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Commands
{
    public record DeleteCommentCommand : IRequest<Result>
    {
        public string CommentId { get; init; }
        public string? DeletedByUserId { get; init; }
    }
}
