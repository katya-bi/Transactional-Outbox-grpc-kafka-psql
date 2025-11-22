namespace TransactionalOutbox.NotificationService.Database.Interfaces;

internal interface ITransactionProvider
{
    Task<ITransaction> CreateTransaction(CancellationToken ct);
}