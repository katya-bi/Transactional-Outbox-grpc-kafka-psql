namespace TransactionalOutbox.OrderService.Kafka.Interfaces;

internal interface IKafkaProducer<TKey, TValue> : IDisposable
{
    Task Produce(IEnumerable<(TKey key, TValue value)> messages, CancellationToken ct);
}