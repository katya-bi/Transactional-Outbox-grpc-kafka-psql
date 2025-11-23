using TransactionalOutbox.NotificationService.Models;

namespace TransactionalOutbox.NotificationService.Database.Repositories.Abstract;

internal interface INotificationRepository
{
    Task<Notification[]> GetNotifications(GetNotifications dto, CancellationToken ct);
    Task SetNotifications(Notification[] notifications, CancellationToken ct);
}