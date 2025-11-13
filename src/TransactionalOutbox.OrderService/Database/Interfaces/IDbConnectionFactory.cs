namespace TransactionalOutbox.OrderService.Database.Interfaces;

internal interface IDbConnectionFactory
{
    Task<IConnection> GetConnection(CancellationToken ct);
}
