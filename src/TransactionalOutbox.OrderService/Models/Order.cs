namespace TransactionalOutbox.OrderService.Models;

internal record Order(Guid UserId, long[] ProductIds, string Status);
