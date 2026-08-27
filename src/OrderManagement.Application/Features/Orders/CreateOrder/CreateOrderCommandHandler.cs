using MediatR;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.RepositoryInterfaces;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _repository;

        public CreateOrderCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Converte os DTOs recebidos pela API em entidades ricas do Domínio
            var domainItems = request.Items
                .Select(item => new OrderItem(item.ProductName, item.Quantity, item.UnitPrice))
                .ToList();

            // Instancia a raiz de agregação do pedido aplicando as regras de negócio
            var order = new Order(request.CustomerId, domainItems);

            // Salva de forma persistente
            await _repository.AddAsync(order, cancellationToken);

            return order.Id;
        }
    }
}
