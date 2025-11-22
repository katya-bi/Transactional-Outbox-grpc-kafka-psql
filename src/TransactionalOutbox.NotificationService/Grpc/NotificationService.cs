using Grpc.Core;
using TransactionalOutbox.Grpc;
using TransactionalOutbox.NotificationService.Services.Abstract;

namespace TransactionalOutbox.NotificationService.Grpc;

internal class NotificationService : TransactionalOutbox.Grpc.NotificationService.NotificationServiceBase
{
    private readonly INotificationService _service;

    public NotificationService(INotificationService service)
    {
        _service = service;
    }

    public override Task<GetUserNotificationsResponse> GetUserNotifications(GetUserNotificationsRequest request, ServerCallContext context)
    {
        return base.GetUserNotifications(request, context);
    }
}