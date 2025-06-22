namespace Nanny.Admin.Application.DTOs;

public class CreateOrderLineDto
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}
