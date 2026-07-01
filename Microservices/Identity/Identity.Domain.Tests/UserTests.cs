using FluentAssertions;
using Identity.Domain.Events;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace Identity.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var result = User.Create("John", "Doe", "john@example.com");

        result.IsSuccess.Should().BeTrue();
        result.Value!.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Email.Should().Be("john@example.com");
        result.Value.UserName.Should().Be("john@example.com");
        result.Value.DomainEvents.Should().HaveCount(1);
        result.Value.DomainEvents.First().Should().BeOfType<UserCreatedDomainEvent>();
    }

    [Fact]
    public void Create_EmptyFirstName_ReturnsFailure()
    {
        var result = User.Create("", "Doe", "john@example.com");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Имя не может быть пустым");
    }

    [Fact]
    public void Create_EmptyLastName_ReturnsFailure()
    {
        var result = User.Create("John", "", "john@example.com");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Фамилия не может быть пустой");
    }

    [Fact]
    public void Create_EmptyEmail_ReturnsFailure()
    {
        var result = User.Create("John", "Doe", "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Email не может быть пустым");
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsFailure()
    {
        var result = User.Create("John", "Doe", "not-an-email");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Некорректный формат Email");
    }

    [Fact]
    public void SetRefreshToken_SetsTokenAndExpiry()
    {
        var user = User.Create("John", "Doe", "john@example.com").Value!;
        var expires = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken("token123", expires);

        user.RefreshToken.Should().Be("token123");
        user.RefreshTokenExpiresAt.Should().Be(expires);
    }

    [Fact]
    public void ClearRefreshToken_ClearsAllTokens()
    {
        var user = User.Create("John", "Doe", "john@example.com").Value!;
        user.SetRefreshToken("token123", DateTime.UtcNow.AddDays(7));

        user.ClearRefreshToken();

        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiresAt.Should().BeNull();
        user.PreviousRefreshToken.Should().BeNull();
        user.PreviousRefreshTokenExpiresAt.Should().BeNull();
    }

    [Fact]
    public void SetRefreshToken_MovesCurrentToPrevious()
    {
        var user = User.Create("John", "Doe", "john@example.com").Value!;
        user.SetRefreshToken("old", DateTime.UtcNow.AddDays(6));

        user.SetRefreshToken("new", DateTime.UtcNow.AddDays(7));

        user.RefreshToken.Should().Be("new");
        user.PreviousRefreshToken.Should().Be("old");
    }

    [Fact]
    public void SetEmail_UpdatesAllEmailFields()
    {
        var user = User.Create("John", "Doe", "old@example.com").Value!;

        user.SetEmail("new@example.com");

        user.Email.Should().Be("new@example.com");
        user.UserName.Should().Be("new@example.com");
        user.NormalizedEmail.Should().Be("NEW@EXAMPLE.COM");
        user.NormalizedUserName.Should().Be("NEW@EXAMPLE.COM");
    }
}