namespace TransactionalOutbox.OrderService.Database.Interfaces;

internal interface IDbMigrator
{
    void Migrate();
}
