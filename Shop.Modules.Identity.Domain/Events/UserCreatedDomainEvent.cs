using Shop.Shared.Domain;

namespace Modules.Identity.Domain.Events;

public sealed record UserCreatedDomainEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}
