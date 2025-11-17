namespace TransactionalOutbox.OrderService.Database.Entities;

internal class OutboxMessageEntity
{
    public long Id { get; set; }
    public string Payload { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}