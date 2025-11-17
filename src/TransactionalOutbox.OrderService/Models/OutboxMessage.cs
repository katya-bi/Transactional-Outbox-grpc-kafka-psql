namespace TransactionalOutbox.OrderService.Models;

internal record OutboxMessage(long Id, string Payload);