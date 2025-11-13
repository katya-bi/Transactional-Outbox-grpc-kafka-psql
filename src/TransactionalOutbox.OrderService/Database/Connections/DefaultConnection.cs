using System.Data.Common;
using TransactionalOutbox.OrderService.Database.Interfaces;

namespace TransactionalOutbox.OrderService.Database.Connections;

internal sealed class DefaultConnection : AbstractConnection
{
    protected override DbConnection Connection { get; }

    public DefaultConnection(DbConnection connection) => Connection = connection;

    public override ValueTask DisposeAsync() => Connection.DisposeAsync();
}
