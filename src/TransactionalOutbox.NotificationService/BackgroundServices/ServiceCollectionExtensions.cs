namespace TransactionalOutbox.NotificationService.BackgroundServices;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services
            .AddHostedService<DbMigrationBackgroundService>();
    }
}