using System.Text;
using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Security.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries;

public class GetAuditLogExportQueryHandler : IRequestHandler<GetAuditLogExportQuery, byte[]>
{
    private readonly MeritociousDbContext context;

    public GetAuditLogExportQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<byte[]> Handle(GetAuditLogExportQuery request, CancellationToken cancellationToken)
    {
        var data = new StringBuilder();

        if (request.IncludeAdminActions)
        {
            var adminActions = await context.AdminActions
                .Where(a => a.Timestamp >= request.StartDate && a.Timestamp <= request.EndDate)
                .OrderBy(a => a.Timestamp)
                .ToListAsync(cancellationToken);

            data.AppendLine("=== Admin Actions ===");
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(adminActions);
                data.AppendLine(writer.ToString());
            }
        }

        if (request.IncludeSecurityEvents)
        {
            var securityEvents = await context.SecurityEvents
                .Where(e => e.Timestamp >= request.StartDate && e.Timestamp <= request.EndDate)
                .OrderBy(e => e.Timestamp)
                .ToListAsync(cancellationToken);

            data.AppendLine("\n=== Security Events ===");
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(securityEvents);
                data.AppendLine(writer.ToString());
            }
        }

        if (request.IncludeLoginAttempts)
        {
            var loginAttempts = await context.LoginAttempts
                .Where(l => l.Timestamp >= request.StartDate && l.Timestamp <= request.EndDate)
                .OrderBy(l => l.Timestamp)
                .ToListAsync(cancellationToken);

            data.AppendLine("\n=== Login Attempts ===");
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(loginAttempts);
                data.AppendLine(writer.ToString());
            }
        }

        if (request.IncludeApiUsage)
        {
            var apiUsage = await context.ApiUsageLogs
                .Where(a => a.Timestamp >= request.StartDate && a.Timestamp <= request.EndDate)
                .OrderBy(a => a.TimeStamp)
                .ToListAsync(cancellationToken);

            data.AppendLine("\n=== API Usage ===");
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(apiUsage);
                data.AppendLine(writer.ToString());
            }
        }

        return Encoding.UTF8.GetBytes(data.ToString());
    }
}