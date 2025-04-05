using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public record GetRelatedTagsQuery : IRequest<Result<List<Tag>>>
    {
        public string TagId { get; init; }
    }
}
