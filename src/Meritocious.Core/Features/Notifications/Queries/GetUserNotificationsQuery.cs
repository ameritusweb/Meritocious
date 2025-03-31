using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Queries
{
    using MediatR;
    using Meritocious.Core.Features.Notifications.Models;

    public record GetUserNotificationsQuery : IRequest<List<Notification>>
    {
        public Guid UserId { get; init; }
        public bool UnreadOnly { get; init; } = false;
        public int? Count { get; init; }
    }
}