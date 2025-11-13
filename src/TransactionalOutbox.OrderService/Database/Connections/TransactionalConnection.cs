using System.Data.Common;
using TransactionalOutbox.OrderService.Database.Interfaces;

namespace TransactionalOutbox.OrderService.Database.Connections;

internal sealed class TransactionalConnection : AbstractConnection
{
    private readonly DbTransaction _transaction;
    protected override DbConnection Connection => _transaction.Connection!;

    public TransactionalConnection(DbTransaction transaction) => _transaction = transaction;

    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

