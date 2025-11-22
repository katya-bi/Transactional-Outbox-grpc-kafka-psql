namespace TransactionalOutbox.NotificationService.Database.Interfaces;

internal interface ITransaction : IAsyncDisposable
{
    Task Commit();
    Task Rollback();
}