using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Nanny.Admin.Application.Services;
using Nanny.Admin.Application.Validators;

namespace Nanny.Admin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
