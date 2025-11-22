namespace TransactionalOutbox.NotificationService.BackgroundServices;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHostedService<OutboxWorker>();
    }
}