using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Database.Repositories;

internal class OrderRepository : IOrderRepository
{
    private const string TableName = "orders";
    private readonly IDbConnectionFactory _connectionFactory;

    public OrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateOrder(Order order, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var parameters = new
        {
            Id = id.ToString(),
            UserId = order.UserId.ToString(),
            ProductIds = order.ProductIds,
            Status = order.Status,
            CreatedAt = DateTimeOffset.UtcNow
        };

        const string sql = $"""
                            INSERT INTO {TableName} (id, user_id, product_ids, status, created_at)
                            VALUES (
                                    @{nameof(parameters.Id)},
                                    @{nameof(parameters.UserId)},
                                    @{nameof(parameters.ProductIds)},
                                    @{nameof(parameters.Status)},
                                    @{nameof(parameters.CreatedAt)})
                            """;

        await using var conn = await _connectionFactory.GetConnection(ct);
        await conn.Execute(sql, parameters, ct);

        return id;
    }
}