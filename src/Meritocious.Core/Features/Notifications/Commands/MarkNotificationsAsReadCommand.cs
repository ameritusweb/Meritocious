using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Commands
{
    using MediatR;
    using Meritocious.Core.Results;

    public record MarkNotificationsAsReadCommand : IRequest<Result>
    {
        public string UserId { get; init; }
        public List<string> NotificationIds { get; init; } = new();
    }
}