using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Enums;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Tests.Domain;

public class OrderTests
{
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly decimal _price = 100.00m;
    private readonly int _count = 2;

    [Theory]
    [InlineData(OrderStatus.Created)]
    [InlineData(OrderStatus.InProgress)]
    public void CanBeModified_ShouldReturnTrue_WhenStatusIsNotCompletedOrCancelled(OrderStatus status)
    {
        // Arrange
        var order = new Order(_customerId);
        typeof(Order).GetProperty("Status")!.SetValue(order, status);

        // Act
        var canBeModified = order.CanBeModified;

        // Assert
        Assert.True(canBeModified);
    }

    [Theory]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void CanBeModified_ShouldReturnFalse_WhenStatusIsCompletedOrCancelled(OrderStatus status)
    {
        // Arrange
        var order = new Order(_customerId);
        typeof(Order).GetProperty("Status")!.SetValue(order, status);

        // Act
        var canBeModified = order.CanBeModified;

        // Assert
        Assert.False(canBeModified);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatusAndUpdatedAt_WhenOrderCanBeModified()
    {
        // Arrange
        var order = new Order(_customerId);
        var newStatus = OrderStatus.InProgress;

        // Act
        order.ChangeStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, order.Status);
        Assert.NotNull(order.UpdatedAt);
        Assert.True(DateTime.UtcNow.Subtract(order.UpdatedAt.Value).TotalSeconds < 1);
    }

    [Theory]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void ChangeStatus_ShouldThrowOrderStateTransitionException_WhenOrderCannotBeModified(OrderStatus initialStatus)
    {
        // Arrange
        var order = new Order(_customerId);
        typeof(Order).GetProperty("Status")!.SetValue(order, initialStatus);

        // Act & Assert
        var exception = Assert.Throws<OrderStateTransitionException>(() =>
            order.ChangeStatus(OrderStatus.InProgress));
        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal(initialStatus, exception.CurrentStatus);
        Assert.Equal(OrderStatus.InProgress, exception.RequestedStatus);
    }

    [Fact]
    public void ChangeStatus_ShouldThrowEmptyOrderException_WhenCompletingEmptyOrder()
    {
        // Arrange
        var order = new Order(_customerId);

        // Act & Assert
        var exception = Assert.Throws<EmptyOrderException>(() => order.ChangeStatus(OrderStatus.Completed));
        Assert.Equal(order.Id, exception.OrderId);
    }

    [Fact]
    public void ChangeStatus_ShouldAllowCompletion_WhenOrderHasItems()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine = new OrderLine(_productId, _count, _price);
        order.AddLine(orderLine);

        // Act
        order.ChangeStatus(OrderStatus.Completed);

        // Assert
        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void AddLine_ShouldAddOrderLineAndUpdateUpdatedAt_WhenOrderCanBeModified()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine = new OrderLine(_productId, _count, _price);

        // Act
        order.AddLine(orderLine);

        // Assert
        Assert.Single(order.OrderLines);
        Assert.Contains(orderLine, order.OrderLines);
        Assert.Equal(_productId, orderLine.ProductId);
        Assert.Equal(_count, orderLine.Count);
        Assert.Equal(_price, orderLine.Price);
        Assert.Null(orderLine.Product);
        Assert.Null(orderLine.Order);
        Assert.NotNull(order.UpdatedAt);
        Assert.True(DateTime.UtcNow.Subtract(order.UpdatedAt.Value).TotalSeconds < 1);
    }

    [Theory]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void AddLine_ShouldThrowInvalidEntityStateException_WhenOrderCannotBeModified(OrderStatus status)
    {
        // Arrange
        var order = new Order(_customerId);
        typeof(Order).GetProperty("Status")!.SetValue(order, status);
        var orderLine = new OrderLine(_productId, _count, _price);

        // Act & Assert
        var exception = Assert.Throws<InvalidEntityStateException>(() =>
            order.AddLine(orderLine));
        Assert.Equal("Order", exception.EntityType);
        Assert.Equal(order.Id, exception.EntityId);
    }

    [Fact]
    public void AddLine_ShouldThrowDuplicateEntityException_WhenAddingDuplicateProduct()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine1 = new OrderLine(_productId, _count, _price);
        var orderLine2 = new OrderLine(_productId, _count + 1, _price);

        // Act
        order.AddLine(orderLine1);

        // Assert
        var exception = Assert.Throws<DuplicateEntityException>(() => order.AddLine(orderLine2));
        Assert.Equal("OrderLine", exception.EntityType);
        Assert.Equal("ProductId", exception.PropertyName);
        Assert.Equal(_productId, exception.PropertyValue);
    }

    [Fact]
    public void RemoveLine_ShouldRemoveOrderLineAndUpdateUpdatedAt_WhenLineExistsAndOrderCanBeModified()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine = new OrderLine(_productId, _count, _price);
        order.AddLine(orderLine);

        // Act
        order.RemoveLine(orderLine.Id);

        // Assert
        Assert.Empty(order.OrderLines);
        Assert.NotNull(order.UpdatedAt);
        Assert.True(DateTime.UtcNow.Subtract(order.UpdatedAt.Value).TotalSeconds < 1);
    }

    [Fact]
    public void RemoveLine_ShouldNotThrow_WhenLineDoesNotExist()
    {
        // Arrange
        var order = new Order(_customerId);

        // Act
        order.RemoveLine(Guid.NewGuid());

        // Assert
        Assert.Empty(order.OrderLines);
        Assert.Null(order.UpdatedAt);
    }

    [Theory]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void RemoveLine_ShouldThrowInvalidEntityStateException_WhenOrderCannotBeModified(OrderStatus status)
    {
        // Arrange
        var order = new Order(_customerId);
        typeof(Order).GetProperty("Status")!.SetValue(order, status);

        // Act & Assert
        var exception = Assert.Throws<InvalidEntityStateException>(() =>
            order.RemoveLine(Guid.NewGuid()));
        Assert.Equal("Order", exception.EntityType);
        Assert.Equal(order.Id, exception.EntityId);
    }

    [Fact]
    public void AddLine_ShouldMaintainMultipleOrderLines()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine1 = new OrderLine(_productId, _count, _price);
        var orderLine2 = new OrderLine(Guid.NewGuid(), _count + 1, _price + 50);

        // Act
        order.AddLine(orderLine1);
        order.AddLine(orderLine2);

        // Assert
        Assert.Equal(2, order.OrderLines.Count);
        Assert.Contains(orderLine1, order.OrderLines);
        Assert.Contains(orderLine2, order.OrderLines);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void GetTotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine1 = new OrderLine(_productId, 2, 10.0m);
        var orderLine2 = new OrderLine(Guid.NewGuid(), 1, 5.0m);
        
        order.AddLine(orderLine1);
        order.AddLine(orderLine2);

        // Act
        var total = order.GetTotalAmount();

        // Assert
        Assert.Equal(25.0m, total); // (2 * 10) + (1 * 5) = 25
    }

    [Fact]
    public void GetTotalAmount_ShouldReturnZero_WhenNoOrderLines()
    {
        // Arrange
        var order = new Order(_customerId);

        // Act
        var total = order.GetTotalAmount();

        // Assert
        Assert.Equal(0.0m, total);
    }

    [Fact]
    public void HasItems_ShouldReturnFalse_WhenNoOrderLines()
    {
        // Arrange
        var order = new Order(_customerId);

        // Act & Assert
        Assert.False(order.HasItems);
    }

    [Fact]
    public void HasItems_ShouldReturnTrue_WhenOrderHasLines()
    {
        // Arrange
        var order = new Order(_customerId);
        var orderLine = new OrderLine(_productId, _count, _price);

        // Act
        order.AddLine(orderLine);

        // Assert
        Assert.True(order.HasItems);
    }
}
