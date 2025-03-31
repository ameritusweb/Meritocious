using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Events
{
    using MediatR;
    using Meritocious.Core.Entities;

    public record ReportResolvedEvent : INotification
    {
        public ContentReport Report { get; }
        public Guid ModeratorId { get; }
        public string Resolution { get; }
        public DateTime ResolvedAt { get; }

        public ReportResolvedEvent(ContentReport report, Guid moderatorId, string resolution)
        {
            Report = report;
            ModeratorId = moderatorId;
            Resolution = resolution;
            ResolvedAt = DateTime.UtcNow;
        }
    }
}