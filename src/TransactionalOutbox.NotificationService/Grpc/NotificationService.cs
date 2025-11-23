using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TransactionalOutbox.Grpc;
using TransactionalOutbox.NotificationService.Models;
using TransactionalOutbox.NotificationService.Services.Abstract;

namespace TransactionalOutbox.NotificationService.Grpc;

internal class NotificationService : TransactionalOutbox.Grpc.NotificationService.NotificationServiceBase
{
    private readonly INotificationService _service;

    public NotificationService(INotificationService service)
    {
        _service = service;
    }

    public override async Task<GetUserNotificationsResponse> GetUserNotifications(GetUserNotificationsRequest request, ServerCallContext context)
    {
        var dto = new GetNotifications(
            UserId: Guid.Parse(request.UserId),
            Limit: request.Limit,
            Offset:  request.Offset
        );
        var notifications = await _service.GetNotifications(dto, context.CancellationToken);
        var notificationsGrpc = notifications
            .Select(n => new GetUserNotificationsResponse.Types.Notification
            {
                Id = n.Id.ToString(),
                OrderId = n.OrderId.ToString(),
                Type = (GetUserNotificationsResponse.Types.Type)n.Type,
                CreatedAt = n.CreatedAt.ToTimestamp()
            });
        return new GetUserNotificationsResponse
        {
            Notifications = { notificationsGrpc }
        };
    }
}