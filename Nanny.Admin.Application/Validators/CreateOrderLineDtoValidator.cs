using FluentValidation;
using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Validators;

public class CreateOrderLineDtoValidator : AbstractValidator<CreateOrderLineDto>
{
    public CreateOrderLineDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.Count)
            .GreaterThan(0).WithMessage("Count must be greater than 0");
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or greater than 0");
    }
}
