namespace TransactionalOutbox.NotificationService.Models;

internal record GetNotifications(Guid UserId, long Limit, long Offset);