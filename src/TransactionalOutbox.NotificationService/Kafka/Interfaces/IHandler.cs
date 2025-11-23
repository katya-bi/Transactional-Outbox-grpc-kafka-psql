using Confluent.Kafka;

namespace TransactionalOutbox.NotificationService.Kafka.Interfaces;

internal interface IHandler<TKey, TValue>
{
    Task Handle(IReadOnlyCollection<ConsumeResult<TKey, TValue>> messages, CancellationToken ct);
}