using TransactionalOutbox.Contracts.Outbox.Models;

namespace TransactionalOutbox.OrderService.Kafka.Interfaces;

internal interface IKafkaProducerFactory
{
    IKafkaProducer<string, OutboxMessagePayload> GetOutboxProducer();
}