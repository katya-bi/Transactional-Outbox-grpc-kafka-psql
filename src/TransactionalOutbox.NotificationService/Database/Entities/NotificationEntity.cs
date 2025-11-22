using TransactionalOutbox.NotificationService.Enums;

namespace TransactionalOutbox.NotificationService.Database.Entities;

internal class NotificationEntity
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string OrderId { get; set; }
    public int Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}