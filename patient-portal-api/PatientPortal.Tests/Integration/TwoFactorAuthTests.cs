using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using PatientPortal.Application.DTOs.Auth;

namespace PatientPortal.Tests.Integration;

/// <summary>
/// Integration tests for Two-Factor Authentication functionality
/// Tests the complete 2FA flow including enable/disable, login with 2FA, code verification, and resend
/// </summary>
public class TwoFactorAuthTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TwoFactorAuthTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Enable2FA_WithAuthenticatedUser_ReturnsSuccess()
    {
        // Arrange
        var (accessToken, _) = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var response = await _client.PostAsync("/api/auth/enable-2fa", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("habilitado");
    }

    [Fact]
    public async Task Disable2FA_WithAuthenticatedUser_ReturnsSuccess()
    {
        // Arrange
        var (accessToken, _) = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Enable 2FA first
        await _client.PostAsync("/api/auth/enable-2fa", null);

        // Act
        var response = await _client.PostAsync("/api/auth/disable-2fa", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("desabilitado");
    }

    [Fact]
    public async Task Get2FAStatus_WithAuthenticatedUser_ReturnsCorrectStatus()
    {
        // Arrange
        var (accessToken, _) = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act - Check status before enabling
        var response1 = await _client.GetAsync("/api/auth/2fa-status");
        var status1 = await response1.Content.ReadFromJsonAsync<TwoFactorStatusDto>();

        // Enable 2FA
        await _client.PostAsync("/api/auth/enable-2fa", null);

        // Check status after enabling
        var response2 = await _client.GetAsync("/api/auth/2fa-status");
        var status2 = await response2.Content.ReadFromJsonAsync<TwoFactorStatusDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        status1.Should().NotBeNull();
        status1!.IsEnabled.Should().BeFalse();

        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        status2.Should().NotBeNull();
        status2!.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Login_With2FAEnabled_ReturnsTwoFactorRequired()
    {
        // Arrange
        var (email, password) = await RegisterUserAsync();
        
        // Login to get access token and enable 2FA
        var loginResponse1 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = password
        });
        var loginResult1 = await loginResponse1.Content.ReadFromJsonAsync<LoginResponseDto>();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult1!.AccessToken);
        await _client.PostAsync("/api/auth/enable-2fa", null);
        _client.DefaultRequestHeaders.Authorization = null;

        // Act - Try to login again (should trigger 2FA)
        var loginResponse2 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = password
        });

        // Assert
        loginResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await loginResponse2.Content.ReadFromJsonAsync<TwoFactorRequiredDto>();
        result.Should().NotBeNull();
        result!.RequiresTwoFactor.Should().BeTrue();
        result.TempToken.Should().NotBeNullOrEmpty();
        result.Message.Should().Contain("verificação");
    }

    [Fact]
    public async Task VerifyTwoFactor_WithInvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var tempToken = await EnableAndTrigger2FAAsync();

        // Act - Verify with invalid code
        var response = await _client.PostAsJsonAsync("/api/auth/verify-2fa", new VerifyTwoFactorDto
        {
            TempToken = tempToken,
            Code = "000000" // Invalid code
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("inválido");
    }

    [Fact]
    public async Task VerifyTwoFactor_WithEmptyCode_ReturnsBadRequest()
    {
        // Arrange
        var tempToken = await EnableAndTrigger2FAAsync();

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/verify-2fa", new VerifyTwoFactorDto
        {
            TempToken = tempToken,
            Code = ""
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("obrigatórios");
    }

    [Fact]
    public async Task VerifyTwoFactor_WithInvalidTempToken_ReturnsBadRequest()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/verify-2fa", new VerifyTwoFactorDto
        {
            TempToken = "invalid-token",
            Code = "123456"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("inválido");
    }

    [Fact]
    public async Task ResendTwoFactorCode_WithValidTempToken_ReturnsSuccess()
    {
        // Arrange
        var tempToken = await EnableAndTrigger2FAAsync();

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/resend-2fa-code", new ResendTwoFactorCodeDto
        {
            TempToken = tempToken
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("reenviado");
    }

    [Fact]
    public async Task ResendTwoFactorCode_WithInvalidTempToken_ReturnsBadRequest()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/resend-2fa-code", new ResendTwoFactorCodeDto
        {
            TempToken = "invalid-token"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendTwoFactorCode_WithEmptyTempToken_ReturnsBadRequest()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/resend-2fa-code", new ResendTwoFactorCodeDto
        {
            TempToken = ""
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        result.Should().NotBeNull();
        result!["message"].Should().Contain("obrigatório");
    }

    [Fact]
    public async Task Enable2FA_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/auth/enable-2fa", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Disable2FA_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/auth/disable-2fa", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get2FAStatus_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/2fa-status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Helper methods

    private async Task<(string accessToken, string email)> RegisterAndLoginUserAsync()
    {
        var email = $"test{Guid.NewGuid()}@example.com";
        var password = "TestPassword123!";

        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Test User",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        return (result!.AccessToken, email);
    }

    private async Task<(string email, string password)> RegisterUserAsync()
    {
        var email = $"test{Guid.NewGuid()}@example.com";
        var password = "TestPassword123!";

        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Test User",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        return (email, password);
    }

    private async Task<string> EnableAndTrigger2FAAsync()
    {
        var (email, password) = await RegisterUserAsync();
        
        // Login to get access token
        var loginResponse1 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = password
        });
        var loginResult1 = await loginResponse1.Content.ReadFromJsonAsync<LoginResponseDto>();
        
        // Enable 2FA
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult1!.AccessToken);
        await _client.PostAsync("/api/auth/enable-2fa", null);
        _client.DefaultRequestHeaders.Authorization = null;

        // Login again to trigger 2FA
        var loginResponse2 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            EmailOrCPF = email,
            Password = password
        });
        var result = await loginResponse2.Content.ReadFromJsonAsync<TwoFactorRequiredDto>();

        return result!.TempToken;
    }

    private static string GenerateRandomCPF()
    {
        var random = new Random();
        var cpf = new int[11];
        
        // Generate first 9 digits
        for (int i = 0; i < 9; i++)
        {
            cpf[i] = random.Next(0, 10);
        }

        // Calculate first check digit
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += cpf[i] * (10 - i);
        }
        int remainder = sum % 11;
        cpf[9] = remainder < 2 ? 0 : 11 - remainder;

        // Calculate second check digit
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += cpf[i] * (11 - i);
        }
        remainder = sum % 11;
        cpf[10] = remainder < 2 ? 0 : 11 - remainder;

        return string.Join("", cpf);
    }
}
