namespace Nanny.Admin.Domain.Exceptions;

public class InvalidEntityStateException : DomainException
{
    public string EntityType { get; }
    public object EntityId { get; }
    public string CurrentState { get; }
    public string RequiredState { get; }

    public InvalidEntityStateException(string entityType, object entityId, string currentState, string requiredState)
        : base($"{entityType} with ID '{entityId}' is in state '{currentState}' but must be in state '{requiredState}' for this operation.")
    {
        EntityType = entityType;
        EntityId = entityId;
        CurrentState = currentState;
        RequiredState = requiredState;
    }

    public InvalidEntityStateException(string entityType, object entityId, string message)
        : base($"{entityType} with ID '{entityId}': {message}")
    {
        EntityType = entityType;
        EntityId = entityId;
        CurrentState = string.Empty;
        RequiredState = string.Empty;
    }
} 