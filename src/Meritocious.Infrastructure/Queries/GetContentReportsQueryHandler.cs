using MediatR;
using Meritocious.Core.Entities;
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
        private readonly IReportingService reportingService;

        public GetContentReportsQueryHandler(IReportingService reportingService)
        {
            this.reportingService = reportingService;
        }

        public async Task<List<ContentReport>> Handle(
            GetContentReportsQuery request,
            CancellationToken cancellationToken)
        {
            return await reportingService.GetReportsAsync(
                request.Status,
                request.SortBy,
                request.Page,
                request.PageSize);
        }
    }
}
