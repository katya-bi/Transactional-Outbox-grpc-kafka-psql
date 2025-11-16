using System.Text.Json;
using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Models;

namespace TransactionalOutbox.OrderService.Database.Repositories;

internal class OutboxRepository : IOutboxRepository
{
    private const string TableName = "outbox_messages";
    private readonly IDbConnectionFactory _connectionFactory;

    public OutboxRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task CreateOutboxMessage(string payload, CancellationToken ct)
    {
        var parameters = new
        {
            Payload = payload,
            CreatedAt = DateTimeOffset.UtcNow
        };

        const string sql = $"""
                            INSERT INTO {TableName} (payload, created_at)
                            VALUES (
                                    @{nameof(parameters.Payload)}::jsonb,
                                    @{nameof(parameters.CreatedAt)})
                            """;

        await using var conn = await _connectionFactory.GetConnection(ct);
        await conn.Execute(sql, parameters, ct);
    }
}