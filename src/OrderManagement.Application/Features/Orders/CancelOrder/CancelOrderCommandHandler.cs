using MediatR;
using OrderManagement.Domain.RepositoryInterfaces;

namespace OrderManagement.Application.Features.Orders.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IOrderRepository _repository;

        public CancelOrderCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Pedido com ID {request.Id} não foi encontrado.");

            // Dispara a regra de transição de estado rica dentro da entidade
            order.Cancel();

            // Atualiza o estado no repositório
            await _repository.UpdateAsync(order, cancellationToken);

            return Unit.Value;
        }
    }
}
