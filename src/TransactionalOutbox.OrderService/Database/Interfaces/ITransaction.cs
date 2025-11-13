namespace TransactionalOutbox.OrderService.Database.Interfaces;

internal interface ITransaction : IAsyncDisposable
{
    Task Commit();
    Task Rollback();
}
