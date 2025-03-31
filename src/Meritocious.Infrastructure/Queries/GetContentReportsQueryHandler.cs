using MediatR;
using Meritocious.Core.Features.Reporting.Queries;
using Meritocious.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public class GetContentReportsQueryHandler : IRequestHandler<GetContentReportsQuery, List<ContentReport>>
    {
        private readonly IReportingService _reportingService;

        public GetContentReportsQueryHandler(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        public async Task<List<ContentReport>> Handle(
            GetContentReportsQuery request,
            CancellationToken cancellationToken)
        {
            return await _reportingService.GetReportsAsync(
                request.Status,
                request.SortBy,
                request.Page,
                request.PageSize);
        }
    }
}
