using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Commands
{
    public record UpdateCommentCommand : IRequest<Result<Comment>>
    {
        public Guid CommentId { get; init; }
        public Guid EditorId { get; init; }
        public string Content { get; init; }
    }
}
