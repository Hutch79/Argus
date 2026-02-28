namespace Argus.Eye.Domain.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
