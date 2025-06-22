using Microsoft.EntityFrameworkCore;
using Nanny.Admin.Application.Common;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Infrastructure.Db;

namespace Nanny.Admin.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync(int page, int pageSize)
    {
        return await context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        return await context.Products.CountAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }
    
    public async Task<bool> UpdateAsync(Product product)
    {
        var existingProduct = await context.Products.FindAsync(product.Id);
        if (existingProduct == null)
        {
            return false;
        }

        context.Products.Update(product);
        await context.SaveChangesAsync();
        return true;
    }
}
