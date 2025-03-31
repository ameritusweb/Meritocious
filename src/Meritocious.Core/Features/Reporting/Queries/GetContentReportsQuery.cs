using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Queries
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Common.Enums;

    public record GetContentReportsQuery : IRequest<List<ContentReport>>
    {
        public string Status { get; init; } = "pending"; // pending, resolved, all
        public string SortBy { get; init; } = "date"; // date, severity, type
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }
}