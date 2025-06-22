namespace Nanny.Admin.Application.DTOs;

public class CreateCustomerDto
{
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? PhotoUrl { get; set; }
}
