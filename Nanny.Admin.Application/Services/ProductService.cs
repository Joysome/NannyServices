using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Exceptions;
using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<PaginatedResult<ProductDto>> GetAllAsync(int page, int pageSize)
    {
        var products = await repository.GetAllAsync(page, pageSize);
        var totalCount = await repository.GetTotalCountAsync();
        
        var items = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        }).ToList();

        return new PaginatedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new ResourceNotFoundException("Product", id);
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }

    public async Task<Guid> CreateAsync(CreateProductDto dto)
    {
        var entity = new Product(dto.Name, dto.Price);
        var created = await repository.CreateAsync(entity);
        return created.Id;
    }

    public async Task UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new ResourceNotFoundException("Product", id);
        }

        product.Update(dto.Name, dto.Price);
        await repository.UpdateAsync(product);
    }
}
