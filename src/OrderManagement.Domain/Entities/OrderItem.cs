namespace OrderManagement.Domain.Entities
{
    public class OrderItem
    {
        // Id e OrderId possuem private set para respeitar o encapsulamento
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        // Construtor privado exigido para materialização do EF Core
        private OrderItem() { }

        // Construtor rico: garante que nenhum item nasça em estado inválido [1]
        public OrderItem(string productName, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(productName));

            if (quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantity));

            if (unitPrice <= 0)
                throw new ArgumentException("O preço unitário deve ser maior que zero.", nameof(unitPrice));

            Id = Guid.NewGuid();
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        // Usado internamente para associar o item ao ID do pedido pai
        internal void AssociateToOrder(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
