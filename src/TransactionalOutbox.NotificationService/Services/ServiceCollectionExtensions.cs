using TransactionalOutbox.NotificationService.Services.Abstract;

namespace TransactionalOutbox.NotificationService.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<INotificationService, NotificationService>();
    }
}