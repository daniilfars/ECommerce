using Shared.Domain;

namespace Modules.Ordering.Domain.Events;

public sealed record OrderCreatedDomainEvent(int Id, Guid UserId) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}