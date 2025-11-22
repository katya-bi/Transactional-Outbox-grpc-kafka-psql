namespace TransactionalOutbox.NotificationService.Database.Interfaces;

internal interface IDbConnectionFactory
{
    Task<IConnection> GetConnection(CancellationToken ct);
}