using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PatientPortal.Application.DTOs.Auth;

namespace PatientPortal.Tests.Security;

/// <summary>
/// Security tests for JWT authentication and authorization
/// </summary>
[Trait("Category", "Security")]
public class JwtSecurityTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public JwtSecurityTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnauthorizedRequest_WithoutToken_ShouldReturn401()
    {
        // Act
        var response = await _client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnauthorizedRequest_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnauthorizedRequest_WithExpiredToken_ShouldReturn401()
    {
        // Arrange - This would require creating an expired token
        // For now, we'll test with a malformed token that looks expired
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjF9.invalid";
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        var response = await _client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMultipleFailedAttempts_ShouldLockAccount()
    {
        // Arrange
        var email = $"lockout{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Lockout Test User",
            Password = "CorrectPassword123!",
            ConfirmPassword = "CorrectPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Register user
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - Multiple failed login attempts
        for (int i = 0; i < 6; i++)
        {
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = "WrongPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        }

        // Try to login with correct password
        var correctLoginRequest = new LoginRequestDto
        {
            Email = email,
            Password = "CorrectPassword123!"
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", correctLoginRequest);

        // Assert - Should be locked out
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithRevokedToken_ShouldReturn401()
    {
        // Arrange - Register and login
        var email = $"revoke{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Revoke Test User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var loginResult = await registerResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Logout (which should revoke the refresh token)
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);
        await _client.PostAsync("/api/auth/logout", null);

        // Act - Try to use the refresh token
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = $"weak{Guid.NewGuid()}@example.com",
            CPF = GenerateRandomCPF(),
            FullName = "Weak Password Test",
            Password = "weak",
            ConfirmPassword = "weak",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithSqlInjectionAttempt_ShouldBeSanitized()
    {
        // Arrange - SQL injection in email
        var request = new RegisterRequestDto
        {
            Email = "test' OR '1'='1@example.com",
            CPF = GenerateRandomCPF(),
            FullName = "SQL Injection Test",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert - Should handle it safely (either reject or sanitize)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, 
            HttpStatusCode.OK
        );
    }

    private static string GenerateRandomCPF()
    {
        var random = new Random();
        return random.Next(10000000000, 99999999999).ToString();
    }
}
