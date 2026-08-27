using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.RepositoryInterfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task UpdateAsync(Order order, CancellationToken cancellationToken);
    }
}
