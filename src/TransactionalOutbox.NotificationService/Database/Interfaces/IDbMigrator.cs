namespace TransactionalOutbox.NotificationService.Database.Interfaces;

internal interface IDbMigrator
{
    void Migrate();
}