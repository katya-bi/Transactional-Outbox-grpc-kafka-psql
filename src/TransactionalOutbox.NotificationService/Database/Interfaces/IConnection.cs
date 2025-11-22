using System.Data.Common;
using Dapper;

namespace TransactionalOutbox.NotificationService.Database.Interfaces;

internal interface IConnection : IAsyncDisposable
{
    Task<int> Execute(string sql, object? param, CancellationToken ct);
    Task<T> QuerySingle<T>(string sql, object? param, CancellationToken ct);
    Task<T?> QuerySingleOrDefault<T>(string sql, object? param, CancellationToken ct);
    Task<IEnumerable<T>> Query<T>(string sql, object? param, CancellationToken ct);
}

internal abstract class AbstractConnection : IConnection
{
    protected abstract DbConnection Connection { get; }

    public Task<int> Execute(string sql, object? param, CancellationToken ct) 
        => Connection.ExecuteAsync(new(sql, param, cancellationToken: ct));

    public Task<T> QuerySingle<T>(string sql, object? param, CancellationToken ct)
        => Connection.QuerySingleAsync<T>(new(sql, param, cancellationToken: ct));

    public Task<T?> QuerySingleOrDefault<T>(string sql, object? param, CancellationToken ct)
        => Connection.QuerySingleOrDefaultAsync<T>(new(sql, param, cancellationToken: ct));

    public Task<IEnumerable<T>> Query<T>(string sql, object? param, CancellationToken ct)
        => Connection.QueryAsync<T>(new(sql, param, cancellationToken: ct));

    public abstract ValueTask DisposeAsync();
}