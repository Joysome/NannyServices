using Nanny.Admin.Application.Common;
using Nanny.Admin.Application.DTOs;
using Nanny.Admin.Application.Exceptions;
using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Application.Services;

public class OrderService(IOrderRepository repository, ICustomerRepository customerRepo, IProductRepository productRepo)
    : IOrderService
{
    public async Task<Guid> CreateAsync(CreateOrderDto dto)
    {
        var customer = await customerRepo.GetByIdAsync(dto.CustomerId);
        if (customer is null)
        {
            throw new ResourceNotFoundException("Customer", dto.CustomerId);
        }

        var order = new Order(dto.CustomerId);

        foreach (var lineDto in dto.OrderLines)
        {
            var product = await productRepo.GetByIdAsync(lineDto.ProductId);
            if (product is null)
            {
                throw new ResourceNotFoundException("Product", lineDto.ProductId);
            }

            var orderLine = new OrderLine(order.Id, product.Id, lineDto.Count, product.Price);
            order.AddLine(orderLine);
        }

        var created = await repository.CreateAsync(order);
        return created.Id;
    }

    public async Task<PaginatedResult<OrderDto>> GetAllAsync(int page, int pageSize)
    {
        var orders = await repository.GetAllAsync(page, pageSize);
        var totalCount = await repository.GetTotalCountAsync();
        
        var items = orders.Select(MapToDto).ToList();

        return new PaginatedResult<OrderDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<OrderDto> GetByIdAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order is null)
        {
            throw new ResourceNotFoundException("Order", id);
        }

        return MapToDto(order);
    }

    public async Task ChangeStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await repository.GetByIdAsync(orderId);
        if (order is null)
        {
            throw new ResourceNotFoundException("Order", orderId);
        }

        order.ChangeStatus(newStatus);
        await repository.UpdateAsync(order);
    }

    public async Task<OrderLineDto> AddOrderLineAsync(Guid orderId, CreateOrderLineDto line)
    {
        var order = await repository.GetByIdAsync(orderId);
        if (order is null)
        {
            throw new ResourceNotFoundException("Order", orderId);
        }

        var product = await productRepo.GetByIdAsync(line.ProductId);
        if (product is null)
        {
            throw new ResourceNotFoundException("Product", line.ProductId);
        }

        var orderLine = new OrderLine(orderId, line.ProductId, line.Count, product.Price);
        order.AddLine(orderLine);

        var success = await repository.UpdateAsync(order);
        if (!success)
        {
            throw new InvalidOperationException("Failed to update order.");
        }

        return new OrderLineDto
        {
            Id = orderLine.Id,
            ProductId = orderLine.ProductId,
            Count = orderLine.Count,
            Price = orderLine.Price
        };
    }

    public async Task RemoveOrderLineAsync(Guid orderId, Guid lineId)
    {
        var order = await repository.GetByIdAsync(orderId);
        if (order is null)
        {
            throw new ResourceNotFoundException("Order", orderId);
        }

        var line = order.OrderLines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
        {
            throw new ResourceNotFoundException("OrderLine", lineId);
        }

        order.RemoveLine(lineId);
        var success = await repository.UpdateAsync(order);
        if (!success)
        {
            throw new InvalidOperationException("Failed to update order.");
        }
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            OrderLines = order.OrderLines.Select(l => new OrderLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                Count = l.Count,
                Price = l.Price
            }).ToList()
        };
    }
}
