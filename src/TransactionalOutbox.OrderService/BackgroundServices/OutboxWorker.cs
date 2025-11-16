using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.OrderService.Kafka.Interfaces;

namespace TransactionalOutbox.OrderService.BackgroundServices;

internal class OutboxWorker : BackgroundService
{
    private readonly IKafkaProducer<string, OutboxMessagePayload> _producer;

    public OutboxWorker(IKafkaProducerFactory producerFactory)
    {
        _producer = producerFactory.GetOutboxProducer();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}