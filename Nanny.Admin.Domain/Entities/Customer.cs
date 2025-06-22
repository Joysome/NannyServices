using Nanny.Admin.Domain.Entities;
using Nanny.Admin.Domain.Exceptions;

public class Customer : BaseEntity
{
    public Customer(string name, string lastName, string address, string? photoUrl = null)
        : base(Guid.NewGuid())
    {
        ValidateCustomerData(name, lastName, address);
        
        Name = name;
        LastName = lastName;
        Address = address;
        PhotoUrl = photoUrl;
    }

    public string Name { get; private set; }
    public string LastName { get; private set; }
    public string Address { get; private set; }
    public string? PhotoUrl { get; private set; }

    public ICollection<Order> Orders { get; private set; } = new List<Order>();

    public void Update(string name, string lastName, string address, string? photoUrl)
    {
        ValidateCustomerData(name, lastName, address);
        
        Name = name;
        LastName = lastName;
        Address = address;
        PhotoUrl = photoUrl;
    }

    private static void ValidateCustomerData(string name, string lastName, string address)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["Name"] = ["Name cannot be null, empty, or whitespace."];
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            errors["LastName"] = ["LastName cannot be null, empty, or whitespace."];
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            errors["Address"] = ["Address cannot be null, empty, or whitespace."];
        }

        if (errors.Any())
        {
            throw new DomainValidationException("Customer", "Customer validation failed", errors);
        }
    }
}
