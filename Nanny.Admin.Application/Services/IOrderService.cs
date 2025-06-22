using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Application.Services;

public interface IOrderService
{
    Task<PaginatedResult<OrderDto>> GetAllAsync(int page, int pageSize);
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateOrderDto dto);
    Task ChangeStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<OrderLineDto> AddOrderLineAsync(Guid orderId, CreateOrderLineDto line);
    Task RemoveOrderLineAsync(Guid orderId, Guid lineId);
}
