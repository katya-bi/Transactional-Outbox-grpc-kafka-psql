using TransactionalOutbox.OrderService.Kafka.Interfaces;
using TransactionalOutbox.OrderService.Kafka.Options;

namespace TransactionalOutbox.OrderService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddOptions(configuration)
            .AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection(nameof(KafkaOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}