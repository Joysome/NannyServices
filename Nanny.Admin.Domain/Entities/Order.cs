using Nanny.Admin.Domain.Enums;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Domain.Entities;

public class Order : BaseEntity
{
    public Order(Guid customerId) : base()
    {
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Created;
    }

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public ICollection<OrderLine> OrderLines { get; private set; } = new List<OrderLine>();

    public bool CanBeModified =>
        Status is not (OrderStatus.Completed or OrderStatus.Cancelled);

    public void ChangeStatus(OrderStatus status)
    {
        if (!CanBeModified)
        {
            throw new OrderStateTransitionException(Id, Status, status, "Order is already completed or cancelled");
        }

        if (status == OrderStatus.Completed && !OrderLines.Any())
        {
            throw new EmptyOrderException(Id);
        }

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLine(OrderLine line)
    {
        if (!CanBeModified)
        {
            throw new InvalidEntityStateException("Order", Id, Status.ToString(), "modifiable state");
        }

        if (OrderLines.Any(x => x.ProductId == line.ProductId))
        {
            throw new DuplicateEntityException("OrderLine", "ProductId", line.ProductId);
        }

        OrderLines.Add(line);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLine(Guid lineId)
    {
        if (!CanBeModified)
        {
            throw new InvalidEntityStateException("Order", Id, Status.ToString(), "modifiable state");
        }

        var line = OrderLines.FirstOrDefault(x => x.Id == lineId);
        if (line != null)
        {
            OrderLines.Remove(line);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public decimal GetTotalAmount()
    {
        return OrderLines.Sum(line => line.Price * line.Count);
    }

    public bool HasItems => OrderLines.Any();
}
