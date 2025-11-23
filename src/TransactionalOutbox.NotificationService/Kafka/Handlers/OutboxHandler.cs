using Confluent.Kafka;
using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.NotificationService.Database.Repositories.Abstract;
using TransactionalOutbox.NotificationService.Kafka.Interfaces;
using TransactionalOutbox.NotificationService.Enums;
using TransactionalOutbox.NotificationService.Models;

namespace TransactionalOutbox.NotificationService.Kafka.Handlers;

internal class OutboxHandler : IHandler<string, OutboxMessagePayload>
{
    private readonly IServiceProvider _serviceProvider;

    public OutboxHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Handle(IReadOnlyCollection<ConsumeResult<string, OutboxMessagePayload>> messages, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();

        var notificationRepository =
            scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        
        var notifications  = messages
            .Select(m => m.Message.Value)
            .Select(omp => new Notification
            (
                Guid.NewGuid(),
                omp.UserId, 
                omp.OrderId, 
                (NotificationsType)omp.Type, 
                DateTimeOffset.UtcNow
            ))
            .ToArray();
        
        return notificationRepository.SetNotifications(notifications, ct);
    }
}