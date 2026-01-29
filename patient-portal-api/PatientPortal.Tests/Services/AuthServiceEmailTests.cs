using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PatientPortal.Application.DTOs.Auth;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using Xunit;

namespace PatientPortal.Tests.Services;

public class AuthServiceEmailTests
{
    private readonly Mock<IPatientUserRepository> _mockPatientUserRepository;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private readonly Mock<IEmailVerificationTokenRepository> _mockEmailVerificationTokenRepository;
    private readonly Mock<IPasswordResetTokenRepository> _mockPasswordResetTokenRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ITwoFactorService> _mockTwoFactorService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceEmailTests()
    {
        _mockPatientUserRepository = new Mock<IPatientUserRepository>();
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _mockEmailVerificationTokenRepository = new Mock<IEmailVerificationTokenRepository>();
        _mockPasswordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockTwoFactorService = new Mock<ITwoFactorService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _mockConfiguration.Setup(x => x["PortalBaseUrl"]).Returns("https://portal.test.com");

        _authService = new AuthService(
            _mockPatientUserRepository.Object,
            _mockRefreshTokenRepository.Object,
            _mockEmailVerificationTokenRepository.Object,
            _mockPasswordResetTokenRepository.Object,
            _mockTokenService.Object,
            _mockNotificationService.Object,
            _mockTwoFactorService.Object,
            _mockConfiguration.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "valid-token";
        
        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            IsUsed = false
        };

        var user = new PatientUser
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User",
            EmailConfirmed = false
        };

        _mockEmailVerificationTokenRepository
            .Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(verificationToken);

        _mockPatientUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPatientUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<PatientUser>()))
            .Returns(Task.CompletedTask);

        _mockEmailVerificationTokenRepository
            .Setup(x => x.UpdateAsync(It.IsAny<EmailVerificationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.VerifyEmailAsync(token);

        // Assert
        result.Should().BeTrue();
        _mockPatientUserRepository.Verify(x => x.UpdateAsync(It.Is<PatientUser>(u => u.EmailConfirmed == true)), Times.Once);
        _mockEmailVerificationTokenRepository.Verify(x => x.UpdateAsync(It.Is<EmailVerificationToken>(t => t.IsUsed == true)), Times.Once);
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var token = "expired-token";
        
        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = Guid.NewGuid(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            IsUsed = false
        };

        _mockEmailVerificationTokenRepository
            .Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(verificationToken);

        // Act
        var result = await _authService.VerifyEmailAsync(token);

        // Assert
        result.Should().BeFalse();
        _mockPatientUserRepository.Verify(x => x.UpdateAsync(It.IsAny<PatientUser>()), Times.Never);
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnFalse_WhenTokenIsAlreadyUsed()
    {
        // Arrange
        var token = "used-token";
        
        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = Guid.NewGuid(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            IsUsed = true // Already used
        };

        _mockEmailVerificationTokenRepository
            .Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(verificationToken);

        // Act
        var result = await _authService.VerifyEmailAsync(token);

        // Assert
        result.Should().BeFalse();
        _mockPatientUserRepository.Verify(x => x.UpdateAsync(It.IsAny<PatientUser>()), Times.Never);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_ShouldSendEmail_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Test User",
            EmailConfirmed = false
        };

        _mockPatientUserRepository
            .Setup(x => x.GetByEmailAsync(email.ToLower()))
            .ReturnsAsync(user);

        _mockEmailVerificationTokenRepository
            .Setup(x => x.CreateAsync(It.IsAny<EmailVerificationToken>()))
            .ReturnsAsync((EmailVerificationToken t) => t);

        _mockNotificationService
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _authService.ResendVerificationEmailAsync(email);

        // Assert
        _mockEmailVerificationTokenRepository.Verify(x => x.CreateAsync(It.IsAny<EmailVerificationToken>()), Times.Once);
        _mockNotificationService.Verify(x => x.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_ShouldThrow_WhenEmailAlreadyVerified()
    {
        // Arrange
        var email = "test@example.com";
        var user = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Test User",
            EmailConfirmed = true // Already verified
        };

        _mockPatientUserRepository
            .Setup(x => x.GetByEmailAsync(email.ToLower()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ResendVerificationEmailAsync(email));
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldSendEmail_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var ipAddress = "127.0.0.1";
        var user = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Test User"
        };

        _mockPatientUserRepository
            .Setup(x => x.GetByEmailAsync(email.ToLower()))
            .ReturnsAsync(user);

        _mockPasswordResetTokenRepository
            .Setup(x => x.CreateAsync(It.IsAny<PasswordResetToken>()))
            .ReturnsAsync((PasswordResetToken t) => t);

        _mockNotificationService
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _authService.RequestPasswordResetAsync(email, ipAddress);

        // Assert
        _mockPasswordResetTokenRepository.Verify(x => x.CreateAsync(It.Is<PasswordResetToken>(
            t => t.PatientUserId == user.Id && t.CreatedByIp == ipAddress)), Times.Once);
        _mockNotificationService.Verify(x => x.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldNotThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var ipAddress = "127.0.0.1";

        _mockPatientUserRepository
            .Setup(x => x.GetByEmailAsync(email.ToLower()))
            .ReturnsAsync((PatientUser?)null);

        // Act
        await _authService.RequestPasswordResetAsync(email, ipAddress);

        // Assert - Should not send email if user doesn't exist (security)
        _mockNotificationService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "valid-reset-token";
        var newPassword = "NewPassword123!";
        
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            IsUsed = false
        };

        var user = new PatientUser
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = "old-hash"
        };

        _mockPasswordResetTokenRepository
            .Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(resetToken);

        _mockPatientUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPatientUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<PatientUser>()))
            .Returns(Task.CompletedTask);

        _mockPasswordResetTokenRepository
            .Setup(x => x.UpdateAsync(It.IsAny<PasswordResetToken>()))
            .Returns(Task.CompletedTask);

        _mockPasswordResetTokenRepository
            .Setup(x => x.RevokeAllActiveTokensAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ResetPasswordAsync(token, newPassword);

        // Assert
        result.Should().BeTrue();
        _mockPatientUserRepository.Verify(x => x.UpdateAsync(It.Is<PatientUser>(u => u.PasswordHash != "old-hash")), Times.Once);
        _mockPasswordResetTokenRepository.Verify(x => x.UpdateAsync(It.Is<PasswordResetToken>(t => t.IsUsed == true)), Times.Once);
        _mockPasswordResetTokenRepository.Verify(x => x.RevokeAllActiveTokensAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var token = "expired-token";
        
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = Guid.NewGuid(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            IsUsed = false
        };

        _mockPasswordResetTokenRepository
            .Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(resetToken);

        // Act
        var result = await _authService.ResetPasswordAsync(token, "NewPassword123!");

        // Assert
        result.Should().BeFalse();
        _mockPatientUserRepository.Verify(x => x.UpdateAsync(It.IsAny<PatientUser>()), Times.Never);
    }
}
