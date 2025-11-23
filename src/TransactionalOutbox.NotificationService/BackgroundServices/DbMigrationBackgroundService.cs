using TransactionalOutbox.NotificationService.Database.Interfaces;

namespace TransactionalOutbox.NotificationService.BackgroundServices;

internal class DbMigrationBackgroundService : BackgroundService
{
    private readonly IDbMigrator _migrator;

    public DbMigrationBackgroundService(IDbMigrator migrator)
    {
        _migrator = migrator;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _migrator.Migrate();
        return Task.CompletedTask;
    }
}