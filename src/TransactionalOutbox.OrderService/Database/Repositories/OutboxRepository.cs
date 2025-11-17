using System.Text.Json;
using TransactionalOutbox.OrderService.Database.Entities;
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

    public async Task<OutboxMessage[]> GetOutboxMessages(int limit, CancellationToken ct)
    {
        var parameters = new
        {
            Limit = limit
        };
        const string sql = $"""
                            SELECT id, payload, created_at, processed_at
                            FROM {TableName}
                            WHERE processed_at IS NULL
                            ORDER BY id
                            FOR UPDATE SKIP LOCKED
                            LIMIT @{nameof(parameters.Limit)}
                            """;
        await using var conn = await _connectionFactory.GetConnection(ct);
        var entities = await conn.Query<OutboxMessageEntity>(sql, parameters, ct);
        return entities.Select(x => new OutboxMessage(x.Id, x.Payload)).ToArray();
    }

    public async Task SetProcessedMessages(long[] ids, CancellationToken ct)
    {
        var parameters = new
        {
            Ids = ids,
            ProcessedAt = DateTimeOffset.UtcNow
        };
        const string sql = $"""
                            UPDATE {TableName}
                            SET processed_at = @{nameof(parameters.ProcessedAt)}
                            WHERE id = ANY(@{nameof(parameters.Ids)})
                            """;
        await using var conn = await _connectionFactory.GetConnection(ct);
        await conn.Execute(sql, parameters, ct);
    }
}