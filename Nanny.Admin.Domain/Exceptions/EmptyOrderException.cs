namespace Nanny.Admin.Domain.Exceptions;

public class EmptyOrderException(Guid orderId)
    : DomainException($"Order '{orderId}' cannot be completed because it has no order lines.")
{
    public Guid OrderId { get; } = orderId;
} 