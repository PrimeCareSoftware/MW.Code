using System.Diagnostics;
using System.Net.Http.Json;
using FluentAssertions;
using PatientPortal.Application.DTOs.Auth;

namespace PatientPortal.Tests.Performance;

/// <summary>
/// Performance tests for authentication endpoints
/// Tests response times and throughput
/// </summary>
[Trait("Category", "Performance")]
public class AuthenticationPerformanceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const int AcceptableResponseTimeMs = 500;
    private const int MaxResponseTimeMs = 2000;

    public AuthenticationPerformanceTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ResponseTime_ShouldBeWithinAcceptableRange()
    {
        // Arrange
        var email = $"perf{Guid.NewGuid()}@example.com";
        var password = "TestPassword123!";

        // Register user first
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Performance Test User",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act - Measure login time
        var loginRequest = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var stopwatch = Stopwatch.StartNew();
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        stopwatch.Stop();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Login should complete within {MaxResponseTimeMs}ms");
    }

    [Fact]
    public async Task Register_ResponseTime_ShouldBeWithinAcceptableRange()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = $"perf{Guid.NewGuid()}@example.com",
            CPF = GenerateRandomCPF(),
            FullName = "Performance Test User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        stopwatch.Stop();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Registration should complete within {MaxResponseTimeMs}ms");
    }

    [Fact]
    public async Task RefreshToken_ResponseTime_ShouldBeWithinAcceptableRange()
    {
        // Arrange
        var email = $"perf{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            CPF = GenerateRandomCPF(),
            FullName = "Performance Test User",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            PhoneNumber = "+55 11 98765-4321",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var loginResult = await registerResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult!.RefreshToken
        };

        // Act
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        stopwatch.Stop();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(AcceptableResponseTimeMs,
            $"Token refresh should complete within {AcceptableResponseTimeMs}ms");
    }

    [Fact]
    public async Task ConcurrentLogins_ShouldHandleLoad()
    {
        // Arrange
        const int concurrentRequests = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Register users
        var users = new List<(string email, string password)>();
        for (int i = 0; i < concurrentRequests; i++)
        {
            var email = $"concurrent{i}-{Guid.NewGuid()}@example.com";
            var password = "TestPassword123!";
            users.Add((email, password));

            var registerRequest = new RegisterRequestDto
            {
                Email = email,
                CPF = GenerateRandomCPF(),
                FullName = $"Concurrent Test User {i}",
                Password = password,
                ConfirmPassword = password,
                PhoneNumber = "+55 11 98765-4321",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        }

        // Act - Concurrent logins
        var stopwatch = Stopwatch.StartNew();
        foreach (var (email, password) in users)
        {
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };
            tasks.Add(_client.PostAsJsonAsync("/api/auth/login", loginRequest));
        }

        var responses = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        responses.Should().AllSatisfy(r => r.IsSuccessStatusCode.Should().BeTrue());
        
        var averageTimePerRequest = stopwatch.ElapsedMilliseconds / concurrentRequests;
        averageTimePerRequest.Should().BeLessThan(MaxResponseTimeMs,
            $"Average response time should be less than {MaxResponseTimeMs}ms under concurrent load");
    }

    [Fact]
    public async Task PasswordHashing_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var password = "TestPassword123!";
        var iterations = 100;
        var times = new List<long>();

        // Act - Multiple password hashing operations
        for (int i = 0; i < iterations; i++)
        {
            var email = $"hash{i}@example.com";
            var request = new RegisterRequestDto
            {
                Email = email,
                CPF = GenerateRandomCPF(),
                FullName = $"Hash Test User {i}",
                Password = password,
                ConfirmPassword = password,
                PhoneNumber = "+55 11 98765-4321",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var stopwatch = Stopwatch.StartNew();
            await _client.PostAsJsonAsync("/api/auth/register", request);
            stopwatch.Stop();
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var averageTime = times.Average();
        var maxTime = times.Max();

        averageTime.Should().BeLessThan(MaxResponseTimeMs,
            $"Average password hashing time should be less than {MaxResponseTimeMs}ms");
        
        maxTime.Should().BeLessThan(MaxResponseTimeMs * 2,
            $"Maximum password hashing time should be less than {MaxResponseTimeMs * 2}ms");
    }

    private static string GenerateRandomCPF()
    {
        var random = new Random();
        return random.Next(10000000000, 99999999999).ToString();
    }
}
