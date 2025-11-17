using System.Text.Json;
using Microsoft.Extensions.Options;
using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.OrderService.BackgroundServices.Options;
using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Kafka.Interfaces;

namespace TransactionalOutbox.OrderService.BackgroundServices;

internal class OutboxWorker : BackgroundService
{
    private readonly IKafkaProducer<string, OutboxMessagePayload> _producer;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<OutboxOptions> _outboxOptions;

    public OutboxWorker(
        IKafkaProducerFactory producerFactory, 
        ILogger<OutboxWorker> logger,
        IOptions<OutboxOptions> outboxOptions, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _outboxOptions = outboxOptions;
        _serviceProvider = serviceProvider;
        _producer = producerFactory.GetOutboxProducer();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(_outboxOptions.Value.SendPeriod);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessBatch(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Outbox processing error: {ex}", ex.Message);
            }
        }
    }

    private async Task ProcessBatch(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var transactionProvider = scope.ServiceProvider.GetRequiredService<ITransactionProvider>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        
        await using var tx = await transactionProvider.CreateTransaction(ct);
        
        var outboxMessages = await outboxRepository.GetOutboxMessages(_outboxOptions.Value.BatchSize, ct);
        
        var kafkaMessages = outboxMessages.Select(x =>
        {
            var payload = JsonSerializer.Deserialize<OutboxMessagePayload>(x.Payload)!;
            return (payload.OrderId.ToString(), payload);
        });
        await _producer.Produce(kafkaMessages, ct);

        var outboxMessageIds = outboxMessages.Select(x => x.Id).ToArray();
        await outboxRepository.SetProcessedMessages(outboxMessageIds, ct);
        
        await tx.Commit();
    }
}