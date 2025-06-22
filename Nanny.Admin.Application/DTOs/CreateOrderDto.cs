namespace Nanny.Admin.Application.DTOs;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; } = [];
}
