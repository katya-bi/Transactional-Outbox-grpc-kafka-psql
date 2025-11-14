using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Database.Repositories.Abstract;

internal interface IOutboxRepository
{
    Task CreateOutboxMessage(OutboxMessage outboxMessage, CancellationToken ct);
}