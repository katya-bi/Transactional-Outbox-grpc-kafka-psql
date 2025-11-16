using System.Text.Json;
using TransactionalOutbox.Contracts.Outbox.Enums;
using TransactionalOutbox.Contracts.Outbox.Models;
using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Models;
using TransactionalOutbox.OrderService.Services.Abstact;

namespace TransactionalOutbox.OrderService.Services;

internal class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ITransactionProvider _transactionProvider;

    public OrderService(
        IOrderRepository orderRepository, 
        IOutboxRepository outboxRepository, 
        ITransactionProvider transactionProvider)
    {
        _orderRepository = orderRepository;
        _outboxRepository = outboxRepository;
        _transactionProvider = transactionProvider;
    }

    public async Task<Guid> CreateOrder(CreateOrder dto, CancellationToken ct)
    {
        await using var tx = await _transactionProvider.CreateTransaction(ct);
        
        var order = new Order(
            dto.UserId,
            dto.ProductIds,
            "Created");
        var orderId = await _orderRepository.CreateOrder(order, ct);

        var outboxMessagePayload = new OutboxMessagePayload
        (
            dto.UserId,
            orderId,
            OutboxMessageType.OrderCreated
        );
        var payload = JsonSerializer.Serialize(outboxMessagePayload);
        await _outboxRepository.CreateOutboxMessage(payload, ct);

        await tx.Commit();
        return orderId;
    }
}