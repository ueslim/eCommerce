using Moq;
using OrderManagement.Application.Features.Orders.CancelOrder;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Application.Features.Orders.GetOrder;
using OrderManagement.Application.Features.Orders.ListOrders;
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

        // ==========================================
        // 1. TESTES DOS COMMANDS (GRAVAÇÃO)
        // ==========================================

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

        // ==========================================
        // 2. TESTES DAS QUERIES (LEITURA)
        // ==========================================

        [Fact]
        public async Task GetOrderQueryHandler_WhenOrderExists_ShouldReturnMappedResponse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), new List<OrderItem> { new("Mouse Gamer", 1, 200m) });

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var handler = new GetOrderQueryHandler(_repositoryMock.Object);
            var query = new GetOrderQuery(orderId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result!.CustomerId);
            Assert.Equal(200m, result.TotalAmount);
            Assert.Single(result.Items);
            Assert.Equal("Mouse Gamer", result.Items[0].ProductName);
        }

        [Fact]
        public async Task GetOrderQueryHandler_WhenOrderDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var handler = new GetOrderQueryHandler(_repositoryMock.Object);
            var query = new GetOrderQuery(orderId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ListOrdersQueryHandler_ShouldReturnPagedResponse()
        {
            // Arrange
            var ordersList = new List<Order>
        {
            new Order(Guid.NewGuid(), new List<OrderItem> { new("Produto A", 1, 10m) }),
            new Order(Guid.NewGuid(), new List<OrderItem> { new("Produto B", 2, 20m) })
        };

            _repositoryMock.Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ordersList, 2));

            var handler = new ListOrdersQueryHandler(_repositoryMock.Object);
            var query = new ListOrdersQuery(1, 10);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(2, result.Items.Count());
        }
    }
}
