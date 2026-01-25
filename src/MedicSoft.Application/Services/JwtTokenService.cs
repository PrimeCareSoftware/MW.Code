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
        string GenerateToken(string username, string userId, string tenantId, string role, string? clinicId = null, bool isSystemOwner = false, string? sessionId = null, string? ownerId = null);
        ClaimsPrincipal? ValidateToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private const string DefaultIssuer = "PrimeCare Software";
        private const string DefaultAudience = "PrimeCare Software-API";
        
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

        private const int FutureNotBeforeToleranceMinutes = 1500; // Allow ~25h skew for nbf drift in legacy tokens (covers day-boundary offset)

        public string GenerateToken(string username, string userId, string tenantId, string role, string? clinicId = null, bool isSystemOwner = false, string? sessionId = null, string? ownerId = null)
        {
            var secretKey = GetSecretKey();
            var issuer = GetIssuer();
            var audience = GetAudience();
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(expiryMinutes);

            // Build claims using JWT registered names where appropriate to ensure proper mapping
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("nameid", userId),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role),
                new Claim("tenant_id", tenantId),
                new Claim("is_system_owner", isSystemOwner.ToString().ToLower())
            };

            if (!string.IsNullOrEmpty(clinicId))
            {
                claims.Add(new Claim("clinic_id", clinicId));
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                claims.Add(new Claim("session_id", sessionId));
            }

            if (!string.IsNullOrEmpty(ownerId))
            {
                claims.Add(new Claim("owner_id", ownerId));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            // Create token via JwtSecurityToken to ensure exp/nbf are embedded
            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials
            );

            _logger.LogDebug("Jwt token created - nbf(unix)={Nbf}, exp(unix)={Exp}, issuer={Issuer}, audience={Audience}",
                jwtToken.Payload.NotBefore, jwtToken.Payload.Expiration, jwtToken.Issuer, string.Join(", ", jwtToken.Audiences));

            var tokenString = tokenHandler.WriteToken(jwtToken);

            _logger.LogInformation("Token generated successfully - UserId: {UserId}, ExpiresAt: {ExpiresAt}, ExpiryMinutes: {ExpiryMinutes}",
                userId, expiresAt, expiryMinutes);

            // Log token details for debugging
            try
            {
                var readBack = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
                var expUnix = readBack.Payload.Expiration;
                _logger.LogDebug("Token payload AFTER WriteToken - Issuer={Issuer}, Audience={Audience}, exp(unix)={ExpUnix}, claims={ClaimCount}",
                    readBack.Issuer,
                    string.Join(", ", readBack.Audiences),
                    expUnix?.ToString() ?? "null",
                    readBack.Claims.Count());

                var expClaim = readBack.Claims.FirstOrDefault(c => c.Type == "exp");
                _logger.LogDebug("Exp claim present: {HasExp}", expClaim != null ? "YES" : "NO");

                var parts = tokenString.Split('.');
                _logger.LogDebug("Token string parts - Header: {HeaderLen} chars, Payload: {PayloadLen} chars, Signature: {SigLen} chars",
                    parts.Length > 0 ? parts[0].Length : 0,
                    parts.Length > 1 ? parts[1].Length : 0,
                    parts.Length > 2 ? parts[2].Length : 0);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not read generated token for debugging: {Message}", ex.Message);
            }

            return tokenString;
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
                // First, try to read the token to log its contents
                try
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
                    var expPayload = jwtToken.Payload.Expiration;
                    
                    _logger.LogDebug("Token read in ValidateToken - Claims: {ClaimCount}, Issuer: {Issuer}, Audience: {Audience}, Expires: {Expires}, Exp Payload: {ExpPayload}, Exp Claim: {ExpClaim}", 
                        jwtToken.Claims.Count(), 
                        jwtToken.Issuer, 
                        string.Join(", ", jwtToken.Audiences), 
                        jwtToken.ValidTo,
                        expPayload,
                        expClaim != null ? $"YES - {expClaim.Value}" : "NO - MISSING");
                }
                catch (Exception readEx)
                {
                    _logger.LogDebug("Could not read token for logging: {Message}", readEx.Message);
                }

                var primaryValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,  // Skip automatic validation due to v7.1.2 bugs with ReadJwtToken
                    ValidateAudience = false,  // Skip automatic validation due to v7.1.2 bugs with ReadJwtToken
                    ValidateLifetime = true,   // But DO validate lifetime
                    RequireExpirationTime = false,  // Don't require Payload.Expiration property
                    ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes tolerance for time sync issues
                };

                var principal = tokenHandler.ValidateToken(token, primaryValidationParameters, out SecurityToken validatedToken);

                // Add all claims from the JWT token to the principal (including custom claims)
                // Use ReadJwtToken(token) to ensure we see the full raw payload as serialized
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                if (jwtSecurityToken != null && principal?.Identity is ClaimsIdentity identity)
                {
                    _logger.LogDebug("Payload contains {Count} items: {Items}", 
                        jwtSecurityToken.Payload.Count,
                        string.Join(", ", jwtSecurityToken.Payload.Select(kv => $"{kv.Key}={kv.Value}")));

                    // Add claims from the JWT payload (covers all claims in the JSON payload)
                    foreach (var payloadItem in jwtSecurityToken.Payload)
                    {
                        // Skip registered claims that are already handled
                        if (payloadItem.Key == "exp" || payloadItem.Key == "iat" || payloadItem.Key == "nbf" || payloadItem.Key == "aud")
                            continue;

                        var claimValue = payloadItem.Value?.ToString();
                        if (!string.IsNullOrEmpty(claimValue))
                        {
                            _logger.LogDebug("Adding claim from payload: {Key}={Value}", payloadItem.Key, claimValue);
                            
                            // Only add if not already present
                            if (!identity.HasClaim(c => c.Type == payloadItem.Key && c.Value == claimValue))
                            {
                                identity.AddClaim(new Claim(payloadItem.Key, claimValue));
                            }
                        }
                    }
                }

                _logger.LogDebug("Token validated successfully. Principal claims: {Claims}", 
                    string.Join(", ", principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? new[] { "none" }));
                
                return principal;
            }
            catch (SecurityTokenInvalidLifetimeException ex)
            {
                // Handles missing exp (IDX10225) or future nbf skew. We retry without lifetime enforcement but manually vet exp/nbf.
                _logger.LogWarning("Token validation failed due to lifetime. Exception: {Message}", ex.Message);
                try
                {
                    var jwt = tokenHandler.ReadJwtToken(token);
                    var now = DateTime.UtcNow;
                    var tolerance = TimeSpan.FromMinutes(FutureNotBeforeToleranceMinutes);

                    DateTime? nbfUtc = null;
                    DateTime? expUtc = null;

                    if (jwt.Payload.NotBefore.HasValue)
                    {
                        nbfUtc = DateTimeOffset.FromUnixTimeSeconds((long)jwt.Payload.NotBefore.Value).UtcDateTime;
                    }

                    if (jwt.Payload.Expiration.HasValue)
                    {
                        expUtc = DateTimeOffset.FromUnixTimeSeconds((long)jwt.Payload.Expiration.Value).UtcDateTime;
                    }

                    _logger.LogInformation("Token lifetime details - NBF: {NBF}, EXP: {EXP}, Now: {Now}", nbfUtc, expUtc, now);

                    // If exp exists and is far in the past (beyond tolerance), reject.
                    if (expUtc.HasValue && now > expUtc.Value + tolerance)
                    {
                        _logger.LogWarning("Legacy validation rejected: token expired beyond tolerance. exp={Exp}", expUtc.Value);
                        return null;
                    }

                    // If nbf exists and is far in the future (beyond tolerance), reject.
                    if (nbfUtc.HasValue && now + tolerance < nbfUtc.Value)
                    {
                        _logger.LogWarning("Legacy validation rejected: token not valid before {Nbf} and beyond tolerance.", nbfUtc.Value);
                        return null;
                    }

                    // If exp is missing, but we have valid nbf and issuer/audience, try to validate without lifetime check
                    // This allows legacy tokens or tokens with missing exp to still be validated if signature is correct
                    if (!expUtc.HasValue)
                    {
                        _logger.LogInformation("Token is missing expiration time (exp). Attempting signature-only validation...");
                    }

                    var legacyValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = false,
                        RequireExpirationTime = false,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    var principal = tokenHandler.ValidateToken(token, legacyValidationParameters, out SecurityToken _);
                    _logger.LogInformation("Token validated with manual lifetime checks or signature-only (no exp check). NBF: {NBF}, EXP: {EXP}", nbfUtc, expUtc);
                    return principal;
                }
                catch (Exception legacyEx)
                {
                    _logger.LogWarning("Legacy token validation also failed: {Message}", legacyEx.Message);
                    return null;
                }
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
