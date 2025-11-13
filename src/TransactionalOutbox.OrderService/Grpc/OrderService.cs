using Grpc.Core;
using TransactionalOutbox.Grpc;
using TransactionalOutbox.OrderService.Models;
using TransactionalOutbox.OrderService.Services.Abstact;

namespace TransactionalOutbox.OrderService.Grpc;

internal class OrderService : TransactionalOutbox.Grpc.OrderService.OrderServiceBase
{
    private readonly IOrderService _service;

    public OrderService(IOrderService service)
    {
        _service = service;
    }
    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var dto = new CreateOrder(
            UserId: Guid.Parse(request.UserId),
            ProductIds: request.ProductIds.ToArray());
        
        var orderId = await _service.CreateOrder(dto, context.CancellationToken);

        return new CreateOrderResponse
        {
            OrderId = orderId.ToString()
        };
    }
}