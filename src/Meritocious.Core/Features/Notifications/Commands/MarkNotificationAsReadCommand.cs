using MediatR;

namespace Meritocious.Core.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand : IRequest<Unit>
{
    public Guid UserId { get; init; }
    public Guid NotificationId { get; init; }
}