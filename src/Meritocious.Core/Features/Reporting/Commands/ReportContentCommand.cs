using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Commands
{
    using MediatR;
    using Meritocious.Common.Enums;
    using Meritocious.Core.Features.Moderation.Commands;
    using Meritocious.Core.Results;
    using Microsoft.Extensions.Logging;

    public record ReportContentCommand : IRequest<Result>
    {
        public string ContentId { get; init; }
        public ContentType ContentType { get; init; }
        public string ReporterId { get; init; }
        public string ReportType { get; init; }
        public string Description { get; init; }
    }
}
