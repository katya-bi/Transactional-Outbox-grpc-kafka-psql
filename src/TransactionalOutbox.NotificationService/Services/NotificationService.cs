using TransactionalOutbox.NotificationService.Database.Repositories.Abstract;
using TransactionalOutbox.NotificationService.Services.Abstract;

namespace TransactionalOutbox.NotificationService.Services;

internal class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
}