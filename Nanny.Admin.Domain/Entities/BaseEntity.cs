namespace Nanny.Admin.Domain.Entities;

public abstract class BaseEntity(Guid id)
{
    public Guid Id { get; private set; } = id;

    protected BaseEntity() : this(Guid.NewGuid())
    {
    }
}
