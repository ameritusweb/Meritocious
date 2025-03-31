using MediatR;
using Meritocious.Core.Features.Notifications.Commands;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class MarkNotificationsAsReadCommandHandler : IRequestHandler<MarkNotificationsAsReadCommand, Result>
    {
        private readonly INotificationService _notificationService;

        public MarkNotificationsAsReadCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(
            MarkNotificationsAsReadCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _notificationService.MarkNotificationsAsReadAsync(
                    request.UserId,
                    request.NotificationIds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
