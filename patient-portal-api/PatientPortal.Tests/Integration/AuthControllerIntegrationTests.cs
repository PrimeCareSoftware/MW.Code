using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PatientPortal.Application.DTOs.Auth;

namespace PatientPortal.Tests.Integration;

/// <summary>
/// Integration tests for Authentication endpoints
/// Tests the complete authentication flow including registration, login, and token refresh
/// </summary>
public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithTokens()
    {
        // Arrange
        var email = $"test{Guid.NewGuid()}@example.com";
        var request = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Test User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(request.Email);
        result.User.FullName.Should().Be(request.FullName);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        
        var firstRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "First User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Register first user
        await _client.PostAsJsonAsync("/api/auth/register", firstRequest);

        // Try to register second user with same email
        var secondRequest = new RegisterRequestDto
        {
            Email = email, // Same email
            CPF = GenerateRandomCPF(), // Different CPF
            FullName = "Second User",
            Password = "TestPassword456!",
            ConfirmPassword = "TestPassword456!",
            PhoneNumber = "+55 11 98765-4322",
            DateOfBirth = new DateTime(1991, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", secondRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange - First register a user
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "TestPassword123!";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Login Test User",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - Now try to login
        var loginRequest = new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange - First register a user
        var email = $"invalid{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Invalid Password Test",
            Password = "CorrectPassword123!",
            ConfirmPassword = "CorrectPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - Try to login with wrong password
        var loginRequest = new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = "WrongPassword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithCPF_ReturnsOkWithTokens()
    {
        // Arrange - First register a user
        var cpf = GenerateRandomCPF();
        var password = "TestPassword123!";
        var registerRequest = new RegisterRequestDto
        {
            Email = $"cpflogin{Guid.NewGuid()}@example.com",
            CPF = cpf,
            FullName = "CPF Login Test",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - Login with CPF instead of email
        var loginRequest = new LoginRequestDto
        {
            EmailOrCPF = cpf,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange - Register and login to get tokens
        var registerRequest = new RegisterRequestDto
        {
            Email = $"refresh{Guid.NewGuid()}@example.com",
            CPF = GenerateRandomCPF(),
            FullName = "Refresh Test User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var loginResult = await registerResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Act - Use refresh token to get new tokens
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult!.RefreshToken
        };

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        // New tokens should be different from old ones
        result.AccessToken.Should().NotBe(loginResult.AccessToken);
        result.RefreshToken.Should().NotBe(loginResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token-12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Helper method to generate a random CPF for testing
    /// Note: This generates a formatted CPF but doesn't validate the checksum
    /// </summary>
    private static string GenerateRandomCPF()
    {
        var random = new Random();
        var cpf = "";
        for (int i = 0; i < 11; i++)
        {
            cpf += random.Next(0, 10).ToString();
        }
        return cpf;
    }
}
