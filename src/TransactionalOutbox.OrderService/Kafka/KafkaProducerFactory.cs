using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.Contracts.Serializers;
using TransactionalOutbox.OrderService.Kafka.Interfaces;
using TransactionalOutbox.OrderService.Kafka.Options;

namespace TransactionalOutbox.OrderService.Kafka;

internal class KafkaProducerFactory : IKafkaProducerFactory
{
    private const string OutboxTopic = "order-outbox";
    private readonly IOptions<KafkaOptions> _options;

    public KafkaProducerFactory(IOptions<KafkaOptions> options)
    {
        _options = options;
    }

    public IKafkaProducer<string, OutboxMessagePayload> GetOutboxProducer()
    {
        return new KafkaProducer<string, OutboxMessagePayload>(
            _options.Value.BootstrapServers,
            OutboxTopic,
            keySerializer: null,
            new SystemTextJsonSerializer<OutboxMessagePayload>(new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));
    }
}