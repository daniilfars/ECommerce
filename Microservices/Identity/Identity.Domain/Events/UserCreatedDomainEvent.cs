using Shared.Domain;

namespace Identity.Domain.Events;

public sealed record UserCreatedDomainEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}
