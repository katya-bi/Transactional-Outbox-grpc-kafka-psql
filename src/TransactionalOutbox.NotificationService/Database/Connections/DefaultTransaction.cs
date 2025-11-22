using System.Data.Common;
using TransactionalOutbox.NotificationService.Database.Interfaces;

namespace TransactionalOutbox.NotificationService.Database.Connections;

internal class DefaultTransaction : ITransaction
{
    private readonly DbConnection _connection;
    private readonly DbTransaction _transaction;

    internal IConnection Connection { get; }

    public event Action? OnDispose;

    public DefaultTransaction(DbConnection connection, DbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
        Connection = new TransactionalConnection(transaction);
    }

    public Task Commit() => _transaction.CommitAsync();

    public Task Rollback() => _transaction.RollbackAsync();

    public async ValueTask DisposeAsync()
    {
        OnDispose?.Invoke();

        await _connection.DisposeAsync();
        await _transaction.DisposeAsync();
    }
}