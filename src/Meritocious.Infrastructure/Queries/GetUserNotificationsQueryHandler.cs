namespace Meritocious.Core.Features.Search.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Notifications;
    using Meritocious.Core.Features.Notifications.Models;
    using Meritocious.Core.Features.Notifications.Queries;
    using Meritocious.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetUserNotificationsQueryHandler
        : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationService _notificationService;

        public GetUserNotificationsQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<List<NotificationDto>> Handle(
            GetUserNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(
                request.UserId,
                request.UnreadOnly,
                request.Count);

            return notifications.ToDtoList();
        }
    }
}