using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Tests.Domain;

public class CustomerTests
{
    [Theory]
    [InlineData(null, "Doe", "123 Main St")]
    [InlineData("", "Doe", "123 Main St")]
    [InlineData("   ", "Doe", "123 Main St")]
    [InlineData("John", null, "123 Main St")]
    [InlineData("John", "", "123 Main St")]
    [InlineData("John", "   ", "123 Main St")]
    [InlineData("John", "Doe", null)]
    [InlineData("John", "Doe", "")]
    [InlineData("John", "Doe", "   ")]
    public void Constructor_WithInvalidParameters_ThrowsDomainValidationException(string name, string lastName,
        string address)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new Customer(name, lastName, address, null));
        Assert.Equal("Customer", exception.EntityType);
        Assert.Contains("Customer validation failed", exception.Message);
    }

    [Theory]
    [InlineData(null, "Smith", "789 Pine Rd")]
    [InlineData("", "Smith", "789 Pine Rd")]
    [InlineData("   ", "Smith", "789 Pine Rd")]
    [InlineData("Jane", null, "789 Pine Rd")]
    [InlineData("Jane", "", "789 Pine Rd")]
    [InlineData("Jane", "   ", "789 Pine Rd")]
    [InlineData("Jane", "Smith", null)]
    [InlineData("Jane", "Smith", "")]
    [InlineData("Jane", "Smith", "   ")]
    public void Update_WithInvalidParameters_ThrowsDomainValidationException(string name, string lastName, string address)
    {
        // Arrange
        var customer = new Customer("John", "Doe", "123 Main St", null);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => customer.Update(name, lastName, address, null));
        Assert.Equal("Customer", exception.EntityType);
        Assert.Contains("Customer validation failed", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesCustomerCorrectly()
    {
        // Arrange & Act
        var customer = new Customer("John", "Doe", "123 Main St", "http://example.com/photo.jpg");

        // Assert
        Assert.Equal("John", customer.Name);
        Assert.Equal("Doe", customer.LastName);
        Assert.Equal("123 Main St", customer.Address);
        Assert.Equal("http://example.com/photo.jpg", customer.PhotoUrl);
        Assert.NotNull(customer.Orders);
        Assert.Empty(customer.Orders);
    }

    [Fact]
    public void Update_ChangesPropertiesCorrectly()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "123 Main St", null);
        var newName = "Jane";
        var newLastName = "Smith";
        var newAddress = "789 Pine Rd";
        var newPhotoUrl = "http://example.com/newphoto.jpg";

        // Act
        customer.Update(newName, newLastName, newAddress, newPhotoUrl);

        // Assert
        Assert.Equal(newName, customer.Name);
        Assert.Equal(newLastName, customer.LastName);
        Assert.Equal(newAddress, customer.Address);
        Assert.Equal(newPhotoUrl, customer.PhotoUrl);
        Assert.NotNull(customer.Orders);
        Assert.Empty(customer.Orders);
    }

    [Fact]
    public void Update_WithNullPhotoUrl_SetsPhotoUrlToNull()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "123 Main St", "http://example.com/photo.jpg");
        var newName = "Jane";
        var newLastName = "Smith";
        var newAddress = "789 Pine Rd";

        // Act
        customer.Update(newName, newLastName, newAddress, null);

        // Assert
        Assert.Equal(newName, customer.Name);
        Assert.Equal(newLastName, customer.LastName);
        Assert.Equal(newAddress, customer.Address);
        Assert.Null(customer.PhotoUrl);
    }
}
