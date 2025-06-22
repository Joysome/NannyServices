using FluentValidation;
using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(dto => dto.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(dto => dto.OrderLines)
            .NotEmpty().WithMessage("At least one order line is required.");
        RuleForEach(dto => dto.OrderLines).SetValidator(new CreateOrderLineDtoValidator());
    }
}
