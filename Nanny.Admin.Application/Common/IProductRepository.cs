using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Application.Common;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
}
