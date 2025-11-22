using TransactionalOutbox.NotificationService.Database.Entities;
using TransactionalOutbox.NotificationService.Database.Interfaces;
using TransactionalOutbox.NotificationService.Database.Repositories.Abstract;
using TransactionalOutbox.NotificationService.Enums;
using TransactionalOutbox.NotificationService.Models;

namespace TransactionalOutbox.NotificationService.Database.Repositories;

internal class NotificationRepository : INotificationRepository
{
    private const string TableName = "notifications";
    private readonly IDbConnectionFactory  _connectionFactory;

    public NotificationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Notification[]> GetNotifications(Guid userId, long limit, long offset, CancellationToken ct)
    {
        var parameters = new
        {
            UserId = userId,
            Limit = limit,
            Offset = offset
        };
        const string sql = $"""
                            SELECT id, user_id, order_id, type, created_at
                            FROM {TableName}
                            WHERE user_id = @{nameof(parameters.UserId)}
                            ORDER BY id
                            LIMIT @{nameof(parameters.Limit)}
                            OFFSET @{nameof(parameters.Offset)}
                            """;
        var conn = await _connectionFactory.GetConnection(ct);
        var entities = await conn.Query<NotificationEntity>(sql, parameters, ct);
        return entities
            .Select(e => new Notification(
                Guid.Parse(e.Id), 
                Guid.Parse(e.UserId), 
                Guid.Parse(e.OrderId), 
                (NotificationsType)e.Type,
                e.CreatedAt))
            .ToArray();
    }

    public async Task SetNotifications(Notification[] notifications, CancellationToken ct)
    {
        var parameters = new
        {
            Ids = notifications.Select(n => n.Id.ToString()).ToArray(),
            UserIds = notifications.Select(n => n.UserId.ToString()).ToArray(),
            OrderIds = notifications.Select(n => n.OrderId.ToString()).ToArray(),
            Types = notifications.Select(n => n.Type).ToArray(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        const string sql = $"""
                            INSERT INTO {TableName} (id, user_id, order_id, type, created_at)
                            SELECT UNNEST(@{nameof(parameters.Ids)}),
                                   UNNEST(@{nameof(parameters.UserIds)}),
                                   UNNEST(@{nameof(parameters.OrderIds)}),
                                   UNNEST(@{nameof(parameters.Types)}),
                                   @{nameof(parameters.CreatedAt)}
                            """;
        
        await using var conn = await _connectionFactory.GetConnection(ct);
        await conn.Execute(sql, parameters, ct);
    }
}