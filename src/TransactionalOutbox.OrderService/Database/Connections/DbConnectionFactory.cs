using Npgsql;
using TransactionalOutbox.OrderService.Database.Interfaces;

namespace TransactionalOutbox.OrderService.Database.Connections;

internal class DbConnectionFactory : IDbConnectionFactory, ITransactionProvider
{
    private DefaultTransaction? _scopedTransaction;
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IConnection> GetConnection(CancellationToken ct)
    {
        if (_scopedTransaction is not null)
        {
            return _scopedTransaction.Connection;
        }
        var conn = await _dataSource.OpenConnectionAsync(ct);
        return new DefaultConnection(conn);
    }

    public async Task<ITransaction> CreateTransaction(CancellationToken ct)
    {
        if (_scopedTransaction is not null)
        {
            return _scopedTransaction!;
        }

        var conn = await _dataSource.OpenConnectionAsync(ct);

        var dbTransaction = await conn.BeginTransactionAsync();
        var transaction = new DefaultTransaction(conn, dbTransaction);

        _scopedTransaction = transaction;
        transaction.OnDispose += () => _scopedTransaction = null;

        return transaction;
    }
}
