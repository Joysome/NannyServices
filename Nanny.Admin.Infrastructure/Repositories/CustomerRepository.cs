using Microsoft.EntityFrameworkCore;
using Nanny.Admin.Application.Common;
using Nanny.Admin.Infrastructure.Db;

namespace Nanny.Admin.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<List<Customer>> GetAllAsync(int page, int pageSize)
    {
        return await context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await context.Customers.CountAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var existingCustomer = await context.Customers.FindAsync(customer.Id);
        if (existingCustomer == null)
        {
            return false;
        }

        context.Customers.Update(customer);
        await context.SaveChangesAsync();
        return true;
    }
}
