using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Exceptions;

namespace Nanny.Admin.Application.Services;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public async Task<PaginatedResult<CustomerDto>> GetAllAsync(int page, int pageSize)
    {
        var customers = await repository.GetAllAsync(page, pageSize);
        var totalCount = await repository.GetTotalCountAsync();
        
        var items = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            LastName = c.LastName,
            Address = c.Address,
            PhotoUrl = c.PhotoUrl
        }).ToList();

        return new PaginatedResult<CustomerDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await repository.GetByIdAsync(id);
        if (customer is null)
        {
            throw new ResourceNotFoundException("Customer", id);
        }

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            LastName = customer.LastName,
            Address = customer.Address,
            PhotoUrl = customer.PhotoUrl
        };
    }

    public async Task<Guid> CreateAsync(CreateCustomerDto dto)
    {
        var entity = new Customer(dto.Name, dto.LastName, dto.Address, dto.PhotoUrl);
        var created = await repository.CreateAsync(entity);
        return created.Id;
    }

    public async Task UpdateAsync(Guid id, CreateCustomerDto dto)
    {
        var customer = await repository.GetByIdAsync(id);
        if (customer is null)
        {
            throw new ResourceNotFoundException("Customer", id);
        }

        customer.Update(dto.Name, dto.LastName, dto.Address, dto.PhotoUrl);
        await repository.UpdateAsync(customer);
    }
}
