using System.Text.Json;
using Nanny.Admin.Application.Exceptions;
using Nanny.Admin.Domain.Exceptions;

namespace Nanny.Admin.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, details) = exception switch
        {
            // Application exceptions
            ResourceNotFoundException => (404, "Resource not found", exception.Message),
            
            // Domain exceptions
            DomainValidationException validationEx => (400, $"{validationEx.EntityType} validation failed", GetValidationDetails(validationEx)),
            InvalidEntityStateException => (409, "Invalid entity state", exception.Message),
            DuplicateEntityException => (409, "Duplicate entity", exception.Message),
            OrderStateTransitionException => (409, "Invalid state transition", exception.Message),
            EmptyOrderException => (400, "Empty order", exception.Message),
            
            // Generic exceptions
            ArgumentException => (400, "Invalid request", exception.Message),
            InvalidOperationException => (400, "Invalid operation", exception.Message),
            KeyNotFoundException => (404, "Resource not found", exception.Message),
            _ => (500, "An unexpected error occurred", exception.Message)
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            status = statusCode,
            message = message,
            details = details,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static object GetValidationDetails(DomainValidationException validationEx)
    {
        if (validationEx.Errors.Any())
        {
            return new
            {
                message = validationEx.Message,
                entityType = validationEx.EntityType,
                errors = validationEx.Errors
            };
        }

        return validationEx.Message;
    }
}
