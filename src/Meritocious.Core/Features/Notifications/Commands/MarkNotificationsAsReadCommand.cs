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
        public Guid UserId { get; init; }
        public List<Guid> NotificationIds { get; init; } = new();
    }
}