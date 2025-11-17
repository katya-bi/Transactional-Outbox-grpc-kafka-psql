using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Database.Repositories.Abstract;

internal interface IOutboxRepository
{
    Task CreateOutboxMessage(string payload, CancellationToken ct);
    Task<OutboxMessage[]> GetOutboxMessages(int limit, CancellationToken ct);
    Task SetProcessedMessages(long[] ids, CancellationToken ct);
}