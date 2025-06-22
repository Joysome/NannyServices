namespace Nanny.Admin.Application.Exceptions;

public class ResourceNotFoundException(string resourceType, object resourceId)
    : ApplicationException($"{resourceType} with ID '{resourceId}' was not found")
{
    public string ResourceType { get; } = resourceType;
    public object ResourceId { get; } = resourceId;
}