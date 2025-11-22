using TransactionalOutbox.NotificationService.Enums;

namespace TransactionalOutbox.NotificationService.Models;

internal record Notification (Guid Id, Guid UserId, Guid OrderId, NotificationsType Type, DateTimeOffset CreatedAt);