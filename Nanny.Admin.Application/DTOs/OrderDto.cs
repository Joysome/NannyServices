using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderLineDto> OrderLines { get; set; } = [];
}
