using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Domain.Exceptions;

public class OrderStateTransitionException : DomainException
{
    public Guid OrderId { get; }
    public OrderStatus CurrentStatus { get; }
    public OrderStatus RequestedStatus { get; }

    public OrderStateTransitionException(Guid orderId, OrderStatus currentStatus, OrderStatus requestedStatus)
        : base($"Cannot transition Order '{orderId}' from '{currentStatus}' to '{requestedStatus}'. This transition is not allowed.")
    {
        OrderId = orderId;
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }

    public OrderStateTransitionException(Guid orderId, OrderStatus currentStatus, OrderStatus requestedStatus, string reason)
        : base($"Cannot transition Order '{orderId}' from '{currentStatus}' to '{requestedStatus}': {reason}")
    {
        OrderId = orderId;
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }
} 