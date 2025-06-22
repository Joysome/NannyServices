using Nanny.Admin.Domain.Enums;

namespace Nanny.Admin.Application.DTOs;

public class ChangeOrderStatusDto(OrderStatus status)
{
    public OrderStatus Status { get; } = status;
}
