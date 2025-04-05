using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Notifications.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Notifications;

    public record GetUserNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public string UserId { get; init; }
        public bool UnreadOnly { get; init; } = false;
        public int? Count { get; init; }
    }
}