﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Commands
{
    using MediatR;
    using Meritocious.Common.Enums;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Moderation.Commands;
    using Meritocious.Core.Results;
    using Microsoft.Extensions.Logging;

    public record ResolveReportCommand : IRequest<Result>
    {
        public string ReportId { get; init; }
        public string ModeratorId { get; init; }
        public string Resolution { get; init; }
        public string Notes { get; init; }
        public ModerationAction Action { get; init; }
    }
}