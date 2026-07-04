using Shared.Domain;

namespace Ordering.Domain.Events;

public sealed record OrderCreatedDomainEvent(Guid Id, Guid UserId) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}