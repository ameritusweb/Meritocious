using MediatR;

namespace Meritocious.Core.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand : IRequest<Unit>
{
    public string UserId { get; init; }
    public string NotificationId { get; init; }
}