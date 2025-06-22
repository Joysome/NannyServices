namespace Nanny.Admin.Domain.Exceptions;

public class DuplicateEntityException(string entityType, string propertyName, object propertyValue)
    : DomainException($"{entityType} with {propertyName} '{propertyValue}' already exists.")
{
    public string EntityType { get; } = entityType;
    public string PropertyName { get; } = propertyName;
    public object PropertyValue { get; } = propertyValue;
} 