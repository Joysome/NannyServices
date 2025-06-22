using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Tests.Domain;

public class ProductTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsDomainValidationException(string invalidName)
    {
        // Arrange
        var price = 10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new Product(invalidName, price));
        Assert.Equal("Product", exception.EntityType);
        Assert.Contains("Product validation failed", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsDomainValidationException()
    {
        // Arrange
        var name = "Test Product";
        var negativePrice = -10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new Product(name, negativePrice));
        Assert.Equal("Product", exception.EntityType);
        Assert.Contains("Product validation failed", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithInvalidName_ThrowsDomainValidationException(string invalidName)
    {
        // Arrange
        var product = new Product("Test Product", 10.00m);
        var price = 20.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => product.Update(invalidName, price));
        Assert.Equal("Product", exception.EntityType);
        Assert.Contains("Product validation failed", exception.Message);
    }

    [Fact]
    public void Update_WithNegativePrice_ThrowsDomainValidationException()
    {
        // Arrange
        var product = new Product("Test Product", 10.00m);
        var name = "New Product";
        var negativePrice = -20.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => product.Update(name, negativePrice));
        Assert.Equal("Product", exception.EntityType);
        Assert.Contains("Product validation failed", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesProductCorrectly()
    {
        // Arrange & Act
        var product = new Product("Test Product", 10.00m);

        // Assert
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(10.00m, product.Price);
    }

    [Fact]
    public void Update_WithValidParameters_UpdatesProductCorrectly()
    {
        // Arrange
        var product = new Product("Test Product", 10.00m);
        var newName = "Updated Product";
        var newPrice = 25.50m;

        // Act
        product.Update(newName, newPrice);

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.Equal(newPrice, product.Price);
    }
}
