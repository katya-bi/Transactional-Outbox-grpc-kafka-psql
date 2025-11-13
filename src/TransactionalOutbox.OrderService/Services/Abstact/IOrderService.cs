using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Services.Abstact;

internal interface IOrderService
{
    Task<Guid> CreateOrder(CreateOrder dto, CancellationToken ct);
}