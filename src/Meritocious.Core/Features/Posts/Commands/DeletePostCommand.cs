using MediatR;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Commands
{
    public record DeletePostCommand : IRequest<Result>
    {
        public string PostId { get; init; }
        public string? DeletedByUserId { get; init; }
    }
}
