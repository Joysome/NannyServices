using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Services;

public interface ICustomerService
{
    Task<PaginatedResult<CustomerDto>> GetAllAsync(int page, int pageSize);
    Task<CustomerDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateCustomerDto dto);
    Task UpdateAsync(Guid id, CreateCustomerDto dto);
}
