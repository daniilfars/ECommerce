using Shared.Domain;

namespace Catalog.Domain.Events;

public sealed record ProductCreatedDomainEvent(int Id, string Name) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}
