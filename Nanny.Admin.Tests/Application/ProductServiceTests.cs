using Moq;
using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Services;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Application.Exceptions;

namespace Nanny.Admin.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Product 1", 10.99m),
            new("Product 2", 20.50m),
            new("Product 3", 15.75m)
        };

        _productRepoMock.Setup(r => r.GetAllAsync(1, 2)).ReturnsAsync(products.Take(2).ToList());
        _productRepoMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(3);

        // Act
        var result = await _productService.GetAllAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, product =>
        {
            Assert.NotNull(product);
            Assert.NotEqual(Guid.Empty, product.Id);
        });
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(new List<Product>());
        _productRepoMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);

        // Act
        var result = await _productService.GetAllAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product("Test Product", 25.99m);
        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(25.99m, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Product);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _productService.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnProductId()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Price = 19.99m
        };

        var createdProduct = new Product("New Product", 19.99m);
        _productRepoMock.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateAsync(createDto);

        // Assert
        Assert.Equal(createdProduct.Id, result);
        _productRepoMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product("Original Product", 10.00m);
        var updateDto = new CreateProductDto
        {
            Name = "Updated Product",
            Price = 15.50m
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

        // Act
        await _productService.UpdateAsync(product.Id, updateDto);

        // Assert
        _productRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updateDto = new CreateProductDto
        {
            Name = "Updated Product",
            Price = 15.50m
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Product);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _productService.UpdateAsync(Guid.NewGuid(), updateDto));
    }
}
