using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Database.Repositories.Abstract;

internal interface IOrderRepository
{
    Task<Guid> CreateOrder(Order order, CancellationToken ct);
}