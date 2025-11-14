using System.Text.Json;
using TransactionalOutbox.OrderService.Database.Interfaces;
using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Enums;
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
        
        var outboxMessagePayload = new
        {
            UserId = dto.UserId,
            OrderId = orderId,
            Type = OutboxMessageType.OrderCreated
        };
        var outboxMessage = new OutboxMessage(JsonSerializer.Serialize(outboxMessagePayload));
        await _outboxRepository.CreateOutboxMessage(outboxMessage, ct);

        await tx.Commit();
        return orderId;
    }
}