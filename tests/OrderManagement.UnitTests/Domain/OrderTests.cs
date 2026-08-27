using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void CreateOrder_WithValidData_ShouldInitializeCorrectlyAndCalculateTotal()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Teclado Mecânico", 2, 150.00m), // Subtotal: 300.00 [1]
            new OrderItem("Mouse Gamer", 1, 250.00m)       // Subtotal: 250.00 [1]
        };

        // Act
        var order = new Order(customerId, items);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(2, order.Items.Count);

        // Regra: TotalAmount deve somar unitPrice * quantity (300 + 250 = 550) [1]
        Assert.Equal(550.00m, order.TotalAmount);
    }

    [Fact]
    public void CreateOrder_WithoutItems_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert - Regra: Pelo menos 1 item é obrigatório [1]
        var customerId = Guid.NewGuid();
        var emptyItems = new List<OrderItem>();

        var exception = Assert.Throws<ArgumentException>(() => new Order(customerId, emptyItems));
        Assert.Contains("pelo menos 1 item", exception.Message);
    }

    [Fact]
    public void CreateOrderItem_WithInvalidQuantity_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert - Regra: Quantidade deve ser maior que zero [1]
        var exception = Assert.Throws<ArgumentException>(() => new OrderItem("Cabo USB", 0, 10.00m));
        Assert.Contains("quantidade deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void CreateOrderItem_WithInvalidUnitPrice_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert - Regra: Preço deve ser maior que zero [1]
        var exception = Assert.Throws<ArgumentException>(() => new OrderItem("Cabo USB", 1, -5.00m));
        Assert.Contains("preço unitário deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void Cancel_WhenOrderIsPending_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem> { new OrderItem("Teclado Mecânico", 1, 150.00m) };
        var order = new Order(customerId, items);

        // Act - Regra: Cancelar pedido pendente [1]
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderIsNotPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem> { new OrderItem("Teclado Mecânico", 1, 150.00m) };
        var order = new Order(customerId, items);

        order.Confirm(); // Muda o status de Pending para Confirmed

        // Act & Assert - Regra: Bloquear cancelamento de não pendentes [1]
        var exception = Assert.Throws<InvalidOperationException>(() => order.Cancel());
        Assert.Contains("Apenas pedidos com status Pendente", exception.Message);
    }
}