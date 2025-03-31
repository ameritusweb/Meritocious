using MediatR;
using Meritocious.Core.Features.Notifications.Models;
using Meritocious.Core.Features.Notifications.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, List<Notification>>
{
    private readonly INotificationService _notificationService;

    public GetUserNotificationsQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<List<Notification>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.GetUserNotificationsAsync(
            request.UserId,
            request.UnreadOnly,
            request.Count);
    }
}