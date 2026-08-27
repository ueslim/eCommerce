using MediatR;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{
    public record CreateOrderCommand(Guid CustomerId, List<CreateOrderItemDto> Items) : IRequest<Guid>;

    public record CreateOrderItemDto(string ProductName, int Quantity, decimal UnitPrice);
}
