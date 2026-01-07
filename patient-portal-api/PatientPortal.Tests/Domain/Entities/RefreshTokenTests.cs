using PatientPortal.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace PatientPortal.Tests.Domain.Entities;

/// <summary>
/// Unit tests for RefreshToken entity
/// </summary>
public class RefreshTokenTests
{
    [Fact]
    public void RefreshToken_NotExpired_ShouldBeActive()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        refreshToken.IsExpired.Should().BeFalse();
        refreshToken.IsRevoked.Should().BeFalse();
        refreshToken.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_Expired_ShouldNotBeActive()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        };

        // Act & Assert
        refreshToken.IsExpired.Should().BeTrue();
        refreshToken.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_Revoked_ShouldNotBeActive()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow,
            RevokedByIp = "127.0.0.1"
        };

        // Act & Assert
        refreshToken.IsRevoked.Should().BeTrue();
        refreshToken.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_WithReplacement_ShouldHaveReplacedByToken()
    {
        // Arrange
        var oldToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "old-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow,
            ReplacedByToken = "new-token",
            ReasonRevoked = "Replaced by new token"
        };

        // Act & Assert
        oldToken.ReplacedByToken.Should().Be("new-token");
        oldToken.ReasonRevoked.Should().Be("Replaced by new token");
        oldToken.IsRevoked.Should().BeTrue();
    }
}
