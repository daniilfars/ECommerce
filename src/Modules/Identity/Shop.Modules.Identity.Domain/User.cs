using Microsoft.AspNetCore.Identity;
using Modules.Identity.Domain.Events;
using Shared.Domain;

namespace Modules.Identity.Domain;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime CreatedAt { get; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public string? PreviousRefreshToken { get; private set; }
    public DateTime? PreviousRefreshTokenExpiresAt { get; private set; }


    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private User() { } // Для EF Core

    private User(string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
        SecurityStamp = Guid.NewGuid().ToString();
        SetEmail(email);
    }

    public static Result<User> Create(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure("Имя не может быть пустым");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure("Фамилия не может быть пустой");

        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure("Email не может быть пустым");

        if (!IsValidEmail(email))
            return Result<User>.Failure("Некорректный формат Email");

        var user = new User(firstName, lastName, email);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Email!));

        return Result<User>.Success(user);
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        PreviousRefreshToken = RefreshToken;
        PreviousRefreshTokenExpiresAt = RefreshTokenExpiresAt;

        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        PreviousRefreshToken = null;
        PreviousRefreshTokenExpiresAt = null;
    }

    public void SetEmail(string email)
    {
        Email = email;
        UserName = email;
        NormalizedEmail = email.ToUpperInvariant();
        NormalizedUserName = email.ToUpperInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}