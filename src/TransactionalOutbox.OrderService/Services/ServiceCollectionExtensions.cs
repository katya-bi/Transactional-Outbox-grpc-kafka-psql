using TransactionalOutbox.OrderService.Services.Abstact;

namespace TransactionalOutbox.OrderService.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IOrderService, OrderService>();
    }
}