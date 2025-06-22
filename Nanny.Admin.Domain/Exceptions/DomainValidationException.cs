namespace Nanny.Admin.Domain.Exceptions;

public class DomainValidationException : DomainException
{
    public string EntityType { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public DomainValidationException(string entityType, string message) : base(message)
    {
        EntityType = entityType;
        Errors = new Dictionary<string, string[]>();
    }

    public DomainValidationException(string entityType, string message, Dictionary<string, string[]> errors) : base(message)
    {
        EntityType = entityType;
        Errors = errors;
    }
} 