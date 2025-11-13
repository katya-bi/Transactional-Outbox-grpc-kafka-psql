namespace TransactionalOutbox.OrderService.Models;

internal record CreateOrder(Guid UserId, long[] ProductIds);