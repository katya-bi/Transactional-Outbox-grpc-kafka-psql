namespace TransactionalOutbox.OrderService.Kafka.Interfaces;

internal interface IKafkaProducer<TKey, TValue> : IDisposable
{
    Task Publish(IEnumerable<(TKey key, TValue value)> messages, CancellationToken ct);
}