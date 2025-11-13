using TransactionalOutbox.OrderService.Database.Repositories.Abstract;
using TransactionalOutbox.OrderService.Models;
using TransactionalOutbox.OrderService.Services.Abstact;

namespace TransactionalOutbox.OrderService.Services;

internal class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> CreateOrder(CreateOrder dto, CancellationToken ct)
    {
        // Открыть транзакцию
        // Добавить заказ в таблицу заказов
        // Добавить заказ в таблицу аутбокса
        // Вернуть di заказа
        var order = new Order(
            dto.UserId,
            dto.ProductIds,
            "Created");
        await _orderRepository.CreateOrder(order, ct);
        return Guid.Empty;
    }
}