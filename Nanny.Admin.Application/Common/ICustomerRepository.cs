namespace Nanny.Admin.Application.Common;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer> CreateAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);
}
