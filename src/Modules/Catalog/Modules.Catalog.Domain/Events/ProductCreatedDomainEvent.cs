using Shared.Domain;

namespace Modules.Catalog.Domain.Events;

public sealed record ProductCreatedDomainEvent(ProductId ProductId, string Name) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}
