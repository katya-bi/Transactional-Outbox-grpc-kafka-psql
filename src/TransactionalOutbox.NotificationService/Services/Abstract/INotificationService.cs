using TransactionalOutbox.NotificationService.Models;

namespace TransactionalOutbox.NotificationService.Services.Abstract;

internal interface INotificationService
{
    Task<Notification[]> GetNotifications(GetNotifications dto, CancellationToken ct);
}