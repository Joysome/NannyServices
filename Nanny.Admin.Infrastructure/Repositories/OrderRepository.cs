using Microsoft.EntityFrameworkCore;
using Nanny.Admin.Application.Common;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Infrastructure.Db;

namespace Nanny.Admin.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<List<Order>> GetAllAsync(int page, int pageSize)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderLines)
            .ThenInclude(ol => ol.Product)
            .OrderBy(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await context.Orders.CountAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await context.Orders
            .Include(o => o.OrderLines)
            .ThenInclude(ol => ol.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> UpdateAsync(Order order)
    {
        var existingOrder = await context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == order.Id);
            
        if (existingOrder == null)
        {
            return false;
        }

        context.Orders.Update(order);
        await context.SaveChangesAsync();
        return true;
    }
}
