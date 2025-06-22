using Moq;
using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;
using Nanny.Admin.Application.Exceptions;
using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Tests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepoMock;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerRepoMock = new Mock<ICustomerRepository>();
        _customerService = new CustomerService(_customerRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new("John", "Doe", "123 Main St", "photo1.jpg"),
            new("Jane", "Smith", "456 Oak Ave", "photo2.jpg"),
            new("Bob", "Johnson", "789 Pine Rd", "photo3.jpg")
        };

        _customerRepoMock.Setup(r => r.GetAllAsync(1, 2)).ReturnsAsync(customers.Take(2).ToList());
        _customerRepoMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(3);

        // Act
        var result = await _customerService.GetAllAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, customer =>
        {
            Assert.NotNull(customer);
            Assert.NotEqual(Guid.Empty, customer.Id);
        });
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(new List<Customer>());
        _customerRepoMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);

        // Act
        var result = await _customerService.GetAllAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "123 Main St", "photo1.jpg");
        _customerRepoMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetByIdAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("123 Main St", result.Address);
        Assert.Equal("photo1.jpg", result.PhotoUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Customer);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _customerService.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnCustomerId()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            Name = "Alice",
            LastName = "Brown",
            Address = "321 Elm St",
            PhotoUrl = "photo4.jpg"
        };

        var createdCustomer = new Customer("Alice", "Brown", "321 Elm St", "photo4.jpg");
        _customerRepoMock.Setup(r => r.CreateAsync(It.IsAny<Customer>())).ReturnsAsync(createdCustomer);

        // Act
        var result = await _customerService.CreateAsync(createDto);

        // Assert
        Assert.Equal(createdCustomer.Id, result);
        _customerRepoMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateCustomer()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "123 Main St", "photo1.jpg");
        var updateDto = new CreateCustomerDto
        {
            Name = "John Updated",
            LastName = "Doe Updated",
            Address = "456 Updated St",
            PhotoUrl = "photo_updated.jpg"
        };

        _customerRepoMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
        _customerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>())).ReturnsAsync(true);

        // Act
        await _customerService.UpdateAsync(customer.Id, updateDto);

        // Assert
        _customerRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updateDto = new CreateCustomerDto
        {
            Name = "John Updated",
            LastName = "Doe Updated",
            Address = "456 Updated St",
            PhotoUrl = "photo_updated.jpg"
        };

        _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Customer);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _customerService.UpdateAsync(Guid.NewGuid(), updateDto));
    }
}
