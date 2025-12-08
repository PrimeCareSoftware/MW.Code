using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MedicSoft.Application.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, string userId, string tenantId, string role, string? clinicId = null, bool isSystemOwner = false, string? sessionId = null);
        ClaimsPrincipal? ValidateToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private const string DefaultIssuer = "MedicWarehouse";
        private const string DefaultAudience = "MedicWarehouse-API";
        
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string GetSecretKey()
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT SecretKey not configured");
            return secretKey;
        }

        private string GetIssuer()
        {
            var issuer = _configuration["JwtSettings:Issuer"];
            return string.IsNullOrWhiteSpace(issuer) ? DefaultIssuer : issuer;
        }

        private string GetAudience()
        {
            var audience = _configuration["JwtSettings:Audience"];
            return string.IsNullOrWhiteSpace(audience) ? DefaultAudience : audience;
        }

        public string GenerateToken(string username, string userId, string tenantId, string role, string? clinicId = null, bool isSystemOwner = false, string? sessionId = null)
        {
            var secretKey = GetSecretKey();
            var issuer = GetIssuer();
            var audience = GetAudience();
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim("tenant_id", tenantId),
                new Claim("is_system_owner", isSystemOwner.ToString().ToLower())
            };

            // Only add clinic_id if it's provided (not for system owners)
            if (!string.IsNullOrEmpty(clinicId))
            {
                claims.Add(new Claim("clinic_id", clinicId));
            }

            // Add session_id if provided
            if (!string.IsNullOrEmpty(sessionId))
            {
                claims.Add(new Claim("session_id", sessionId));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("ValidateToken called with null or empty token");
                return null;
            }

            // Strip "Bearer " prefix if present
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) && token.Length > 7)
            {
                token = token[7..];
                _logger.LogDebug("Stripped 'Bearer ' prefix from token");
            }

            var secretKey = GetSecretKey();
            var issuer = GetIssuer();
            var audience = GetAudience();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes tolerance for time sync issues
                }, out SecurityToken validatedToken);

                _logger.LogDebug("Token validated successfully");
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token validation failed: Token has expired. Exception: {Message}", ex.Message);
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogWarning("Token validation failed: Invalid signature. Exception: {Message}", ex.Message);
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return null;
            }
        }
    }
}
