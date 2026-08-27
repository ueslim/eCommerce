using MediatR;
using OrderManagement.Domain.RepositoryInterfaces;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    // 1. O Contrato de Entrada (Query)
    public record GetOrderQuery(Guid Id) : IRequest<OrderResponse?>;

    // 2. Os DTOs de Saída (Evita expor a entidade de domínio pura para o cliente HTTP)
    public record OrderResponse(
        Guid Id,
        Guid CustomerId,
        string Status,
        DateTime CreatedAt,
        decimal TotalAmount,
        List<OrderResponseItem> Items
    );

    public record OrderResponseItem(
        Guid Id,
        string ProductName,
        int Quantity,
        decimal UnitPrice
    );

    // 3. O Handler da Query
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResponse?>
    {
        private readonly IOrderRepository _repository;

        public GetOrderQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderResponse?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (order == null)
                return null;

            // Mapeamento manual de forma performática
            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.CreatedAt,
                order.TotalAmount,
                order.Items.Select(item => new OrderResponseItem(
                    item.Id,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice
                )).ToList()
            );
        }
    }
}
