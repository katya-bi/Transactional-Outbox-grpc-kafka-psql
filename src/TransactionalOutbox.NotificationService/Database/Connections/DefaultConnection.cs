using System.Data.Common;
using TransactionalOutbox.NotificationService.Database.Interfaces;

namespace TransactionalOutbox.NotificationService.Database.Connections;

internal sealed class DefaultConnection : AbstractConnection
{
    protected override DbConnection Connection { get; }

    public DefaultConnection(DbConnection connection) => Connection = connection;

    public override ValueTask DisposeAsync() => Connection.DisposeAsync();
}