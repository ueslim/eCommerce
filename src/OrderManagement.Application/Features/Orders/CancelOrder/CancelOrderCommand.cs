using MediatR;

namespace OrderManagement.Application.Features.Orders.CancelOrder
{
    public record CancelOrderCommand(Guid Id) : IRequest<Unit>;
}
