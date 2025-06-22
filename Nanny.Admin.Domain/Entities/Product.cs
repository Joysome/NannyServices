using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Domain.Entities;

public class Product : BaseEntity
{
    public Product(string name, decimal price) : base()
    {
        ValidateProductData(name, price);
        
        Name = name;
        Price = price;
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public void Update(string name, decimal price)
    {
        ValidateProductData(name, price);
        
        Name = name;
        Price = price;
    }

    private static void ValidateProductData(string name, decimal price)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["Name"] = ["Name cannot be null, empty, or whitespace."];
        }

        if (price < 0)
        {
            errors["Price"] = ["Price cannot be negative."];
        }

        if (errors.Any())
        {
            throw new DomainValidationException("Product", "Product validation failed", errors);
        }
    }
}
