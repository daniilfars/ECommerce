using Shared.Domain;

namespace Modules.Ordering.Domain.Events;

public sealed record OrderCreatedDomainEvent(int Id, int UserId) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}