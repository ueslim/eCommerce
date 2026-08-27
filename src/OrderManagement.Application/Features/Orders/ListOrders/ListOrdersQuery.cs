using MediatR;
using OrderManagement.Domain.RepositoryInterfaces;

namespace OrderManagement.Application.Features.Orders.ListOrders
{
    // 1. O Contrato de Entrada (Query com Paginação)
    public record ListOrdersQuery(int Page = 1, int PageSize = 10) : IRequest<PagedOrdersResponse>;

    // 2. O DTO do Resultado Paginado
    public record PagedOrdersResponse(
        IEnumerable<MiniOrderResponse> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages
    );

    public record MiniOrderResponse(
        Guid Id,
        Guid CustomerId,
        string Status,
        DateTime CreatedAt,
        decimal TotalAmount,
        int TotalItems
    );

    // 3. O Handler da Query
    public class ListOrdersQueryHandler : IRequestHandler<ListOrdersQuery, PagedOrdersResponse>
    {
        private readonly IOrderRepository _repository;

        public ListOrdersQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedOrdersResponse> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
        {
            // Garante paginação segura
            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            // Busca os dados paginados no repositório
            var (orders, totalCount) = await _repository.GetPagedAsync(page, pageSize, cancellationToken);

            // Mapeia para um DTO simplificado e performático para listagem
            var items = orders.Select(order => new MiniOrderResponse(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.CreatedAt,
                order.TotalAmount,
                order.Items.Count
            ));

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedOrdersResponse(items, page, pageSize, totalCount, totalPages);
        }
    }
}
