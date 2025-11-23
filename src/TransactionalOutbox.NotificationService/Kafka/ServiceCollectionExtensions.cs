using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Polly;
using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.Contracts.Serializers;
using TransactionalOutbox.NotificationService.Kafka.Handlers;
using TransactionalOutbox.NotificationService.Kafka.Interfaces;
using TransactionalOutbox.NotificationService.Kafka.Options;

namespace TransactionalOutbox.NotificationService.Kafka;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddOptions(configuration)
            .AddRetryPolicy()
            .AddKafkaConsumer<string, OutboxMessagePayload, OutboxHandler>("order-outbox");
    }

    private static IServiceCollection AddRetryPolicy(this IServiceCollection services)
    {
        return services.AddSingleton<IAsyncPolicy>(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Kafka));

            return Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    _ => TimeSpan.FromSeconds(1),
                    (exception, ts) =>
                    {
                        logger.LogWarning(exception, "Kafka retry after {Delay}", ts);
                    });
        });
    }

    private static IServiceCollection AddKafkaConsumer<TKey, TValue, THandler>(
        this IServiceCollection services,
        string settingsName)
        where THandler : class, IHandler<TKey, TValue>
    {
        services.AddSingleton<IHandler<TKey, TValue>, THandler>();
        services.AddHostedService(sp => new KafkaConsumer<TKey, TValue>(
            sp.GetRequiredService<IOptionsMonitor<KafkaOptions>>(),
            sp.GetRequiredService<IOptionsMonitor<KafkaConsumerSettings>>(),
            sp.GetRequiredService<IHandler<TKey, TValue>>(),
            sp.GetRequiredService<IAsyncPolicy>(),
            null,
            new SystemTextJsonSerializer<TValue>(new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }),
            sp.GetRequiredService<ILogger<KafkaConsumer<TKey, TValue>>>(),
            settingsName
        ));

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection(nameof(KafkaOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<KafkaConsumerSettings>("order-outbox")
            .Bind(configuration.GetSection($"{nameof(KafkaConsumerSettings)}:OrderOutbox"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }
}