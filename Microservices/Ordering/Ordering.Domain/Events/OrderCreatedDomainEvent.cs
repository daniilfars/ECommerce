using Shared.Domain;

namespace Ordering.Domain.Events;

public sealed record OrderCreatedDomainEvent(int Id, Guid UserId) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}