namespace Shop.Shared.Domain;

public interface IDomainEvent
{
    DateTime OccuredAt { get; }
}