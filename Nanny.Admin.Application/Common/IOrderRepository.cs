using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Application.Common;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Order order);
    Task<bool> UpdateAsync(Order order);
}
