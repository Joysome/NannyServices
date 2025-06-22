using FluentValidation;
using Nanny.Admin.Application.DTOs;

namespace Nanny.Admin.Application.Validators;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(c => c.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(250);

        RuleFor(c => c.PhotoUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("PhotoUrl must be a valid URL or empty");
    }
}
