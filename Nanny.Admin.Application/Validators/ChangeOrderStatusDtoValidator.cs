using FluentValidation;
using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Validators;

public class ChangeOrderStatusDtoValidator : AbstractValidator<ChangeOrderStatusDto>
{
    public ChangeOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status");
    }
}
