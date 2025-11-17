using Confluent.Kafka;
using TransactionalOutbox.OrderService.Kafka.Interfaces;

namespace TransactionalOutbox.OrderService.Kafka;

internal sealed class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;
    private const int ChunkSize = 100;

    public KafkaProducer(
        string bootstrapServers,
        string topic,
        ISerializer<TKey>? keySerializer,
        ISerializer<TValue>? valueSerializer)
    {
        _topic = topic;

        var builder = new ProducerBuilder<TKey, TValue>(
            new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            });

        if (keySerializer is not null)
        {
            builder.SetKeySerializer(keySerializer);
        }

        if (valueSerializer is not null)
        {
            builder.SetValueSerializer(valueSerializer);
        }

        _producer = builder.Build();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }

    public async Task Produce(IEnumerable<(TKey key, TValue value)> messages, CancellationToken ct)
    {
        var tasks = new List<Task>(ChunkSize);

        foreach (var (key, value) in messages)
        {
            tasks.Add(
                _producer.ProduceAsync(
                    _topic,
                    new Message<TKey, TValue> { Key = key, Value = value },
                    ct));
            
            if (tasks.Count == ChunkSize)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }
        
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }
}
