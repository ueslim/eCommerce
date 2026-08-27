using Moq;
using OrderManagement.Application.Features.Orders.CancelOrder;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.RepositoryInterfaces;

namespace OrderManagement.UnitTests.Features
{
    public class OrderHandlersTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;

        public OrderHandlersTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
        }

        [Fact]
        public async Task CreateOrderHandler_WithValidCommand_ShouldSaveAndReturnOrderId()
        {
            // Arrange
            var handler = new CreateOrderCommandHandler(_repositoryMock.Object);
            var command = new CreateOrderCommand(
                Guid.NewGuid(),
                new List<CreateOrderItemDto> { new("Teclado Mecânico", 1, 150.00m) }
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrderHandler_WhenOrderExistsAndIsPending_ShouldCancelSuccessfully()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var pendingOrder = new Order(Guid.NewGuid(), new List<OrderItem> { new("Mouse", 1, 100m) });

            // Simulamos o retorno do banco trazendo o pedido pendente
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pendingOrder);

            var handler = new CancelOrderCommandHandler(_repositoryMock.Object);
            var command = new CancelOrderCommand(orderId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, pendingOrder.Status);
            _repositoryMock.Verify(r => r.UpdateAsync(pendingOrder, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
