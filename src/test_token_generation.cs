// Script para validar que tokens são gerados corretamente com a claim 'exp'
// Execute com: dotnet script test_token_generation.cs

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

// Simular o JwtTokenService
public class TestJwtTokenService
{
    private const string DefaultIssuer = "PrimeCare Software";
    private const string DefaultAudience = "PrimeCare Software-API";
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestJwtTokenService> _logger;

    public TestJwtTokenService(IConfiguration configuration, ILogger<TestJwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(string username, string userId, string tenantId, string role, 
        string? clinicId = null, bool isSystemOwner = false, string? sessionId = null)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? "DefaultSecretKey-MustBe-AtLeast32Characters-ForSecurity!";
        var issuer = DefaultIssuer;
        var audience = DefaultAudience;
        var expiryMinutes = 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(expiryMinutes);

        // Convert to Unix timestamps for the exp and nbf claims
        var expiresAtUnixTime = ((DateTimeOffset)expiresAt).ToUnixTimeSeconds();
        var nowUnixTime = ((DateTimeOffset)now).ToUnixTimeSeconds();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role),
            new Claim("tenant_id", tenantId),
            new Claim("is_system_owner", isSystemOwner.ToString().ToLower()),
            // Explicitly add exp claim to ensure it's in the token
            new Claim("exp", expiresAtUnixTime.ToString(), ClaimValueTypes.Integer64),
            // Explicitly add nbf claim for consistency
            new Claim("nbf", nowUnixTime.ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrEmpty(clinicId))
        {
            claims.Add(new Claim("clinic_id", clinicId));
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            claims.Add(new Claim("session_id", sessionId));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);
        
        // Validate the token has exp
        var jwtToken = tokenHandler.ReadJwtToken(tokenString);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        
        Console.WriteLine($"✓ Token generated successfully");
        Console.WriteLine($"  - Has exp claim: {(expClaim != null ? "YES" : "NO")}");
        Console.WriteLine($"  - Exp value: {expClaim?.Value}");
        Console.WriteLine($"  - Total claims: {jwtToken.Claims.Count()}");
        Console.WriteLine($"  - Payload.Expiration: {jwtToken.Payload.Expiration}");

        return tokenString;
    }

    public bool ValidateToken(string token)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? "DefaultSecretKey-MustBe-AtLeast32Characters-ForSecurity!";
        var issuer = DefaultIssuer;
        var audience = DefaultAudience;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            Console.WriteLine($"✓ Token validation succeeded");
            Console.WriteLine($"  - Subject: {principal?.Identity?.Name}");
            return true;
        }
        catch (SecurityTokenInvalidLifetimeException ex)
        {
            Console.WriteLine($"✗ Token validation failed (lifetime): {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Token validation failed: {ex.Message}");
            return false;
        }
    }
}

// Main test
Console.WriteLine("=== Testing JWT Token Generation and Validation ===\n");

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        {"JwtSettings:SecretKey", "TestSecretKey-MustBe-AtLeast32Characters-ForSecurity!"},
        {"JwtSettings:Issuer", "PrimeCare Software"},
        {"JwtSettings:Audience", "PrimeCare Software-API"},
        {"JwtSettings:ExpiryMinutes", "60"}
    })
    .Build();

var logger = LoggerFactory.Create(builder => builder.AddConsole())
    .CreateLogger<TestJwtTokenService>();

var service = new TestJwtTokenService(config, logger);

// Generate token
Console.WriteLine("1. Generating token...");
var token = service.GenerateToken(
    username: "testuser",
    userId: Guid.NewGuid().ToString(),
    tenantId: "test-tenant",
    role: "Doctor"
);
Console.WriteLine();

// Validate token
Console.WriteLine("2. Validating token...");
var isValid = service.ValidateToken(token);
Console.WriteLine();

if (isValid)
{
    Console.WriteLine("✓ SUCCESS: Token was generated and validated successfully!");
}
else
{
    Console.WriteLine("✗ FAILED: Token validation failed!");
}
