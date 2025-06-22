using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Tests.Domain;

public class OrderLineTests
{
    [Fact]
    public void Constructor_SetsBaseEntityId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var count = 3;
        var price = 29.99m;

        // Act
        var orderLine = new OrderLine(productId, count, price);

        // Assert
        Assert.IsType<Guid>(orderLine.Id);
        Assert.NotEqual(Guid.Empty, orderLine.Id);
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var count = 5;
        var price = 19.99m;

        // Act
        var orderLine = new OrderLine(productId, count, price);

        // Assert
        Assert.NotEqual(Guid.Empty, orderLine.Id);
        Assert.Equal(productId, orderLine.ProductId);
        Assert.Equal(count, orderLine.Count);
        Assert.Equal(price, orderLine.Price);
        Assert.Null(orderLine.Order);
        Assert.Null(orderLine.Product);
        Assert.Equal(Guid.Empty, orderLine.OrderId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCount_ThrowsDomainValidationException(int invalidCount)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var price = 25.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new OrderLine(productId, invalidCount, price));
        Assert.Equal("OrderLine", exception.EntityType);
        Assert.Contains("OrderLine validation failed", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsDomainValidationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var count = 1;
        var negativePrice = -10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new OrderLine(productId, count, negativePrice));
        Assert.Equal("OrderLine", exception.EntityType);
        Assert.Contains("OrderLine validation failed", exception.Message);
    }

    [Fact]
    public void Constructor_WithOrderId_InitializesOrderIdCorrectly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var count = 2;
        var price = 15.00m;

        // Act
        var orderLine = new OrderLine(orderId, productId, count, price);

        // Assert
        Assert.Equal(orderId, orderLine.OrderId);
        Assert.Equal(productId, orderLine.ProductId);
        Assert.Equal(count, orderLine.Count);
        Assert.Equal(price, orderLine.Price);
    }
}
