using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Reporting.Events
{
    using MediatR;
    using Meritocious.Core.Entities;

    public record ContentReportedEvent : INotification
    {
        public ContentReport Report { get; }

        public ContentReportedEvent(ContentReport report)
        {
            Report = report;
        }
    }
}