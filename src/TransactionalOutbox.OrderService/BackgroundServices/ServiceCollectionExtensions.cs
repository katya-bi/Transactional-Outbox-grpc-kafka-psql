using TransactionalOutbox.OrderService.BackgroundServices.Options;

namespace TransactionalOutbox.OrderService.BackgroundServices;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddOptions(configuration)
            .AddHostedService<DbMigrationBackgroundService>()
            .AddHostedService<OutboxWorker>();
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<OutboxOptions>()
            .Bind(configuration.GetSection(nameof(OutboxOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}
