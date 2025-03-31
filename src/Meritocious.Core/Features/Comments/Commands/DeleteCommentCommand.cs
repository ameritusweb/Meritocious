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
        public Guid CommentId { get; init; }
        public Guid? DeletedByUserId { get; init; }
    }
}
