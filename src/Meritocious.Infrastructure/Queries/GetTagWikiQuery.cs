using MediatR;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public class GetTagWikiQuery : IRequest<Result<TagWikiDto>>
    {
        public string TagId { get; set; }
    }
}
