using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Encapsulamento da lista para evitar manipulação externa direta (ex: Order.Items.Add() burlaria validações)
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // Regra: O valor total deve ser calculado de forma dinâmica no domínio [1]
        public decimal TotalAmount => _items.Sum(item => item.Quantity * item.UnitPrice);
        // Construtor vazio para o EF Core
        private Order() { }

        // Construtor de negócio
        public Order(Guid customerId, List<OrderItem> items)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("O ID do cliente não pode ser vazio.", nameof(customerId));

            // Regra: Um pedido deve ter pelo menos 1 item [1]
            if (items == null || !items.Any())
                throw new ArgumentException("Um pedido deve ter pelo menos 1 item.", nameof(items));

            Id = Guid.NewGuid();
            CustomerId = customerId;
            Status = OrderStatus.Pending; // Nasce como Pendente por padrão [1]
            CreatedAt = DateTime.UtcNow;

            // Associa os itens de forma íntegra
            foreach (var item in items)
            {
                item.AssociateToOrder(Id);
                _items.Add(item);
            }
        }

        // Regra de negócio: Apenas pedidos com status Pending podem ser cancelados [1]
        public void Cancel()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Apenas pedidos com status Pendente podem ser cancelados.");

            Status = OrderStatus.Cancelled;
        }

        // Regra de negócio para apoiar o fluxo (Confirmação de pedido)
        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Apenas pedidos Pendentes podem ser confirmados.");

            Status = OrderStatus.Confirmed;
        }
    }
}
