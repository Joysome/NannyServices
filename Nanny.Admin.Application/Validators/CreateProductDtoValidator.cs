using FluentValidation;
using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MinimumLength(2).WithMessage("Product name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Product name must be at most 200 characters");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
