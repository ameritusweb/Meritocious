using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Merit.Commands
{
    using MediatR;
    using Meritocious.Common.Enums;
    using Meritocious.Core.Events;
    using Meritocious.Core.Interfaces;
    using Meritocious.Core.Results;

    public record RecalculateMeritScoreCommand : IRequest<Result<decimal>>
    {
        public Guid ContentId { get; init; }
        public ContentType ContentType { get; init; }
    }
}