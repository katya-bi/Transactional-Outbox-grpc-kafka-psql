using TransactionalOutbox.NotificationService.Models;

namespace TransactionalOutbox.NotificationService.Database.Repositories.Abstract;

internal interface INotificationRepository
{
    Task<Notification[]> GetNotifications(Guid userId, long limit, long offset, CancellationToken ct);
    Task SetNotifications(Notification[] notifications, CancellationToken ct);
}