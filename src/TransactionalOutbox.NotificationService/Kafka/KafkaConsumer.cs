using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Polly;
using TransactionalOutbox.NotificationService.Kafka.Interfaces;
using TransactionalOutbox.NotificationService.Kafka.Options;

namespace TransactionalOutbox.NotificationService.Kafka;

internal class KafkaConsumer<TKey, TValue> : BackgroundService {
    private readonly KafkaConsumerSettings _settings;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<KafkaConsumer<TKey, TValue>> _logger;

    public KafkaConsumer(IOptionsMonitor<KafkaOptions> kafkaOptions,
        IOptionsMonitor<KafkaConsumerSettings> settings,
        IHandler<TKey, TValue> handler,
        IAsyncPolicy retryPolicy,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer,
        ILogger<KafkaConsumer<TKey, TValue>> logger, 
        string settingsName)
    {
        _settings = settings.Get(settingsName);
        _handler = handler;
        _retryPolicy = retryPolicy;
        _logger = logger;

        var cfg = kafkaOptions.CurrentValue;

        var builder = new ConsumerBuilder<TKey, TValue>(new ConsumerConfig
        {
            BootstrapServers = cfg.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = false
        });

        if (keyDeserializer != null) builder.SetKeyDeserializer(keyDeserializer);
        if (valueDeserializer != null) builder.SetValueDeserializer(valueDeserializer);

        _consumer = builder.Build();
        _consumer.Subscribe(_settings.Topic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        _logger.LogInformation("Kafka consumer reading topic {Topic}", _settings.Topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var msg = _consumer.Consume(stoppingToken);

            _logger.LogTrace("[{Topic}] {Partition}:{Offset} consumed",
                _settings.Topic,
                msg.Partition.Value,
                msg.Offset.Value);

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _handler.Handle([msg], stoppingToken);
                _consumer.StoreOffset(msg);
            });
        }
        
        _consumer.Close();
        _consumer.Dispose();
    }
}
