using TransactionalOutbox.OrderService.Database.Interfaces;

namespace TransactionalOutbox.OrderService.BackgroundServices;

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
