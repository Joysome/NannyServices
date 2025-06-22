using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Domain.Entities;

public class OrderLine : BaseEntity
{
    public OrderLine(Guid productId, int count, decimal price) : base()
    {
        ValidateOrderLineData(count, price);
        
        ProductId = productId;
        Count = count;
        Price = price;
    }

    public OrderLine(Guid orderId, Guid productId, int count, decimal price) : this(productId, count, price)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Count { get; private set; }
    public decimal Price { get; private set; }

    private static void ValidateOrderLineData(int count, decimal price)
    {
        var errors = new Dictionary<string, string[]>();

        if (count <= 0)
        {
            errors["Count"] = ["Count must be greater than zero."];
        }

        if (price < 0)
        {
            errors["Price"] = ["Price cannot be negative."];
        }

        if (errors.Any())
        {
            throw new DomainValidationException("OrderLine", "OrderLine validation failed", errors);
        }
    }
}
