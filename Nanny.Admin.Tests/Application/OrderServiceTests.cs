using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Enums;
using Moq;
using Nanny.Admin.Application.Exceptions;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService(_orderRepoMock.Object, _customerRepoMock.Object, _productRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_CreateOrder_When_ValidData()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "Address");
        var product = new Product("Toy", 10m);

        _customerRepoMock.Setup(r => r.GetByIdAsync(customer.Id))
            .ReturnsAsync(customer);
        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var createOrderDto = new CreateOrderDto
        {
            CustomerId = customer.Id,
            OrderLines = [new CreateOrderLineDto { ProductId = product.Id, Count = 2, Price = 10m }]
        };

        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var resultId = await _orderService.CreateAsync(createOrderDto);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);
        _orderRepoMock.Verify(r => r.CreateAsync(It.Is<Order>(o =>
            o.CustomerId == customer.Id &&
            o.OrderLines.Count == 1 &&
            o.OrderLines.First().ProductId == product.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_CustomerNotFound()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Customer);
        var dto = new CreateOrderDto { CustomerId = Guid.NewGuid(), OrderLines = [] };

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _orderService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_ProductNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _customerRepoMock.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(new Customer("John", "Doe", "Address"));
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Product);

        var dto = new CreateOrderDto
        {
            CustomerId = customerId,
            OrderLines = [new CreateOrderLineDto { ProductId = Guid.NewGuid(), Count = 1, Price = 10m }]
        };

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _orderService.CreateAsync(dto));
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnOrderDto_When_Found()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.AddLine(new OrderLine(Guid.NewGuid(), 1, 20m));

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var dto = await _orderService.GetByIdAsync(order.Id);

        // Assert
        Assert.Equal(order.Id, dto.Id);
        Assert.NotNull(dto.OrderLines);
        Assert.Single(dto.OrderLines);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_When_NotFound()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Order);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _orderService.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_UpdateStatus_When_OrderExists()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.AddLine(new OrderLine(Guid.NewGuid(), 1, 20m));
        
        var orderId = order.Id;
        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

        var newStatus = OrderStatus.Completed;

        // Act
        await _orderService.ChangeStatusAsync(orderId, newStatus);

        // Assert
        Assert.Equal(newStatus, order.Status);
        _orderRepoMock.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Throw_When_OrderNotFound()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Order);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            _orderService.ChangeStatusAsync(Guid.NewGuid(), OrderStatus.Completed));
    }

    [Fact]
    public async Task AddOrderLineAsync_Should_AddLine_When_Valid()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        var orderId = order.Id;
        var product = new Product("Toy", 15m);
        var productId = product.Id;
        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _orderRepoMock.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

        var lineDto = new CreateOrderLineDto { ProductId = productId, Count = 1, Price = 15m };

        // Act
        await _orderService.AddOrderLineAsync(orderId, lineDto);

        // Assert
        Assert.Contains(order.OrderLines, l => l.ProductId == productId);
        _orderRepoMock.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    [Fact]
    public async Task AddOrderLineAsync_Should_Throw_When_ProductAlreadyAdded()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 10m);
        var order = new Order(Guid.NewGuid());
        
        // Create an order line with the same product ID that we'll try to add again
        var existingOrderLine = new OrderLine(productId, 1, 10m);
        order.AddLine(existingOrderLine);
        var orderId = order.Id;

        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var dto = new CreateOrderLineDto { ProductId = productId, Count = 1, Price = 10m };

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _orderService.AddOrderLineAsync(orderId, dto));
    }

    [Fact]
    public async Task AddOrderLineAsync_Should_Throw_When_ProductNotFound()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        var orderId = order.Id;
        var productId = Guid.NewGuid();

        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(null as Product);

        var dto = new CreateOrderLineDto { ProductId = productId, Count = 1, Price = 10m };

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _orderService.AddOrderLineAsync(orderId, dto));
    }
}
