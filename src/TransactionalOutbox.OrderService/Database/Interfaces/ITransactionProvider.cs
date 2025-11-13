namespace TransactionalOutbox.OrderService.Database.Interfaces;

internal interface ITransactionProvider
{
    Task<ITransaction> CreateTransaction(CancellationToken ct);
}
