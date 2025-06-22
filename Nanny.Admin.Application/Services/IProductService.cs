using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Services;

public interface IProductService
{
    Task<PaginatedResult<ProductDto>> GetAllAsync(int page, int pageSize);
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateProductDto dto);
    Task UpdateAsync(Guid id, CreateProductDto dto);
}
