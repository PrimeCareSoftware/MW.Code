using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Authentication controller for user and owner login
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint for regular users (doctors, secretaries, etc.)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("User login attempt with null request body");
                    return BadRequest(new { message = "Dados de login não fornecidos." });
                }

                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("User login attempt with missing credentials. Username: {Username}", 
                        request.Username ?? "null");
                    return BadRequest(new { message = "Nome de usuário e senha são obrigatórios." });
                }

                // Get tenantId from request or from middleware context
                var tenantId = request.TenantId;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    tenantId = HttpContext.Items["TenantId"] as string;
                    if (string.IsNullOrWhiteSpace(tenantId))
                    {
                        _logger.LogWarning("User login attempt without tenantId and no tenant context found");
                        return BadRequest(new { message = "Identificador da clínica não encontrado. Por favor, acesse através do domínio da clínica." });
                    }
                    _logger.LogInformation("Using tenantId from context: {TenantId}", tenantId);
                }

                _logger.LogInformation("User login attempt for username: {Username}, tenantId: {TenantId}", 
                    request.Username, tenantId);

                var user = await _authService.AuthenticateUserAsync(
                    request.Username, 
                    request.Password, 
                    tenantId
                );

                if (user == null)
                {
                    _logger.LogWarning("Failed user login attempt for username: {Username}, tenantId: {TenantId}", 
                        request.Username, tenantId);
                    return Unauthorized(new { message = "Usuário ou senha incorretos." });
                }

                _logger.LogInformation("User authenticated successfully: {UserId}, username: {Username}", 
                    user.Id, user.Username);

                // Record login and get session ID
                string sessionId;
                try
                {
                    sessionId = await _authService.RecordUserLoginAsync(user.Id, tenantId);
                    _logger.LogInformation("User login recorded for: {UserId} with session: {SessionId}", user.Id, sessionId);
                }
                catch (Exception recordEx)
                {
                    // Log but don't fail the login if recording fails
                    _logger.LogError(recordEx, "Failed to record user login for: {UserId}", user.Id);
                    return StatusCode(500, new { message = "Não foi possível registrar a sessão de login. Por favor, tente novamente." });
                }

                // Generate JWT token with session ID
                var token = _jwtTokenService.GenerateToken(
                    username: user.Username,
                    userId: user.Id.ToString(),
                    tenantId: tenantId,
                    role: user.Role.ToString(),
                    clinicId: user.ClinicId?.ToString(),
                    sessionId: sessionId
                );

                _logger.LogInformation("JWT token generated successfully for user: {UserId}", user.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    TenantId = tenantId,
                    Role = user.Role.ToString(),
                    ClinicId = user.ClinicId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user login for username: {Username}, tenantId: {TenantId}", 
                    request?.Username ?? "unknown", request?.TenantId ?? "unknown");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar o login. Por favor, tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Login endpoint for owners (clinic owners and system owners)
        /// </summary>
        [HttpPost("owner-login")]
        public async Task<ActionResult<LoginResponse>> OwnerLogin([FromBody] LoginRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Owner login attempt with null request body");
                    return BadRequest(new { message = "Dados de login não fornecidos." });
                }

                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Owner login attempt with missing credentials. Username: {Username}", 
                        request.Username ?? "null");
                    return BadRequest(new { message = "Nome de usuário e senha são obrigatórios." });
                }

                // Get tenantId from request or from middleware context
                var tenantId = request.TenantId;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    tenantId = HttpContext.Items["TenantId"] as string;
                    if (string.IsNullOrWhiteSpace(tenantId))
                    {
                        _logger.LogWarning("Owner login attempt without tenantId and no tenant context found");
                        return BadRequest(new { message = "Identificador da clínica não encontrado. Por favor, acesse através do domínio da clínica." });
                    }
                    _logger.LogInformation("Using tenantId from context: {TenantId}", tenantId);
                }

                _logger.LogInformation("Owner login attempt for username: {Username}, tenantId: {TenantId}", 
                    request.Username, tenantId);

                // Authenticate owner
                var owner = await _authService.AuthenticateOwnerAsync(
                    request.Username, 
                    request.Password, 
                    tenantId
                );

                if (owner == null)
                {
                    _logger.LogWarning("Failed owner login attempt for username: {Username}, tenantId: {TenantId}", 
                        request.Username, tenantId);
                    return Unauthorized(new { message = "Usuário ou senha incorretos." });
                }

                _logger.LogInformation("Owner authenticated successfully: {OwnerId}, username: {Username}", 
                    owner.Id, owner.Username);

                // Record login and get session ID
                string sessionId;
                try
                {
                    sessionId = await _authService.RecordOwnerLoginAsync(owner.Id, tenantId);
                    _logger.LogInformation("Owner login recorded for: {OwnerId} with session: {SessionId}", owner.Id, sessionId);
                }
                catch (Exception recordEx)
                {
                    // Log but don't fail the login if recording fails
                    _logger.LogError(recordEx, "Failed to record owner login for: {OwnerId}", owner.Id);
                    return StatusCode(500, new { message = "Não foi possível registrar a sessão de login. Por favor, tente novamente." });
                }

                // Generate JWT token with session ID
                var token = _jwtTokenService.GenerateToken(
                    username: owner.Username,
                    userId: owner.Id.ToString(),
                    tenantId: tenantId,
                    role: RoleNames.ClinicOwner,
                    clinicId: owner.ClinicId?.ToString(),
                    isSystemOwner: owner.IsSystemOwner,
                    sessionId: sessionId
                );

                _logger.LogInformation("JWT token generated successfully for owner: {OwnerId}", owner.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = owner.Username,
                    TenantId = tenantId,
                    Role = RoleNames.ClinicOwner,
                    ClinicId = owner.ClinicId,
                    IsSystemOwner = owner.IsSystemOwner,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during owner login for username: {Username}, tenantId: {TenantId}", 
                    request?.Username ?? "unknown", request?.TenantId ?? "unknown");
                return StatusCode(500, new { message = "An error occurred during login. Please try again later." });
            }
        }

        /// <summary>
        /// Validate if a token is still valid
        /// </summary>
        [HttpPost("validate")]
        public ActionResult<TokenValidationResponse> ValidateToken([FromBody] TokenValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var principal = _jwtTokenService.ValidateToken(request.Token);

            if (principal == null)
            {
                return Ok(new TokenValidationResponse { IsValid = false });
            }

            var username = principal.Identity?.Name;
            var role = principal.FindFirst("role")?.Value
                ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var tenantId = principal.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(tenantId))
            {
                try
                {
                    // Strip Bearer prefix if present
                    var tokenString = request.Token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) && request.Token.Length > 7
                        ? request.Token[7..]
                        : request.Token;

                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var raw = handler.ReadJwtToken(tokenString);

                    username ??= raw.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value
                        ?? raw.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                    role ??= raw.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                        ?? raw.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                    tenantId ??= raw.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
                }
                catch
                {
                    // ignore fallback errors; will return what we have
                }
            }

            return Ok(new TokenValidationResponse
            {
                IsValid = true,
                Username = username,
                Role = role,
                TenantId = tenantId
            });
        }

        /// <summary>
        /// Validate if the current session is still active
        /// </summary>
        [HttpPost("validate-session")]
        public async Task<ActionResult<SessionValidationResponse>> ValidateSession([FromBody] SessionValidationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    return BadRequest(new { message = "Token is required" });
                }

                _logger.LogDebug("ValidateSession - Token received, length: {TokenLength}, first 50 chars: {TokenStart}", 
                    request.Token.Length, 
                    request.Token.Length > 50 ? request.Token.Substring(0, 50) : request.Token);

                var principal = _jwtTokenService.ValidateToken(request.Token);
                if (principal == null)
                {
                    _logger.LogWarning("ValidateSession failed: Token validation returned null");
                    return Ok(new SessionValidationResponse 
                    { 
                        IsValid = false,
                        Message = "Token inválido ou expirado"
                    });
                }

                // Extract claims - try both mapped names and JWT short names
                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                    ?? principal.FindFirst("nameid")?.Value 
                    ?? principal.FindFirst("sub")?.Value;
                var sessionIdClaim = principal.FindFirst("session_id")?.Value;
                var tenantIdClaim = principal.FindFirst("tenant_id")?.Value;
                var roleClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value 
                    ?? principal.FindFirst("role")?.Value;

                // Fallback: if any required custom claim is missing, parse raw JWT payload directly
                if (string.IsNullOrWhiteSpace(sessionIdClaim) || string.IsNullOrWhiteSpace(tenantIdClaim) || string.IsNullOrWhiteSpace(userIdClaim))
                {
                    try
                    {
                        var tokenString = request.Token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) && request.Token.Length > 7
                            ? request.Token[7..]
                            : request.Token;

                        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var raw = handler.ReadJwtToken(tokenString);

                        userIdClaim ??= raw.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? raw.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value
                            ?? raw.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                        sessionIdClaim ??= raw.Claims.FirstOrDefault(c => c.Type == "session_id")?.Value;
                        tenantIdClaim ??= raw.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

                        _logger.LogInformation("ValidateSession - Fallback parsed claims: UserId={UserId}, SessionId={SessionId}, TenantId={TenantId}",
                            userIdClaim ?? "null", sessionIdClaim ?? "null", tenantIdClaim ?? "null");

                        // As a last resort, decode the JWT payload manually to read raw JSON
                        if (string.IsNullOrWhiteSpace(sessionIdClaim) || string.IsNullOrWhiteSpace(tenantIdClaim) || string.IsNullOrWhiteSpace(userIdClaim))
                        {
                            var parts = tokenString.Split('.');
                            if (parts.Length >= 2)
                            {
                                string? payloadJson = null;
                                try
                                {
                                    payloadJson = Base64UrlDecode(parts[1]);
                                    using var doc = JsonDocument.Parse(payloadJson);
                                    var root = doc.RootElement;
                                    if (string.IsNullOrWhiteSpace(userIdClaim))
                                    {
                                        if (root.TryGetProperty("nameid", out var nameidProp)) userIdClaim = nameidProp.GetString();
                                        else if (root.TryGetProperty("sub", out var subProp)) userIdClaim = subProp.GetString();
                                    }
                                    if (string.IsNullOrWhiteSpace(sessionIdClaim) && root.TryGetProperty("session_id", out var sessProp))
                                    {
                                        sessionIdClaim = sessProp.GetString();
                                    }
                                    if (string.IsNullOrWhiteSpace(tenantIdClaim) && root.TryGetProperty("tenant_id", out var tenantProp))
                                    {
                                        tenantIdClaim = tenantProp.GetString();
                                    }

                                    _logger.LogInformation("ValidateSession - Manual JWT decode claims: UserId={UserId}, SessionId={SessionId}, TenantId={TenantId}",
                                        userIdClaim ?? "null", sessionIdClaim ?? "null", tenantIdClaim ?? "null");
                                }
                                catch (Exception decodeEx)
                                {
                                    _logger.LogWarning(decodeEx, "Manual JWT payload decode failed");
                                }
                            }
                        }
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogWarning(parseEx, "ValidateSession fallback parse failed");
                    }
                }

                static string Base64UrlDecode(string input)
                {
                    string padded = input.Replace('-', '+').Replace('_', '/');
                    switch (padded.Length % 4)
                    {
                        case 2: padded += "=="; break;
                        case 3: padded += "="; break;
                    }
                    var bytes = Convert.FromBase64String(padded);
                    return Encoding.UTF8.GetString(bytes);
                }

                _logger.LogInformation("ValidateSession - Claims extracted: UserId={UserId}, SessionId={SessionId}, TenantId={TenantId}, Role={Role}",
                    userIdClaim ?? "null", sessionIdClaim ?? "null", tenantIdClaim ?? "null", roleClaim ?? "null");

                if (string.IsNullOrWhiteSpace(userIdClaim) || 
                    string.IsNullOrWhiteSpace(tenantIdClaim))
                {
                    _logger.LogWarning("ValidateSession failed: Missing userId or tenantId claims");
                    return Ok(new SessionValidationResponse 
                    { 
                        IsValid = false,
                        Message = "Token inválido"
                    });
                }

                // If sessionId is missing, it might be an old token - still validate token expiry
                if (string.IsNullOrWhiteSpace(sessionIdClaim))
                {
                    _logger.LogInformation("ValidateSession: No session_id claim found in token, treating as valid (legacy token)");
                    return Ok(new SessionValidationResponse 
                    { 
                        IsValid = true,
                        Message = "Sessão válida (token sem session_id)"
                    });
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Ok(new SessionValidationResponse 
                    { 
                        IsValid = false,
                        Message = "Token inválido"
                    });
                }

                bool isSessionValid;
                
                // Check if this is an owner or regular user
                if (roleClaim == "Owner" || roleClaim == RoleNames.ClinicOwner)
                {
                    isSessionValid = await _authService.ValidateOwnerSessionAsync(userId, sessionIdClaim, tenantIdClaim);
                }
                else
                {
                    isSessionValid = await _authService.ValidateUserSessionAsync(userId, sessionIdClaim, tenantIdClaim);
                }

                if (!isSessionValid)
                {
                    _logger.LogWarning("Session validation failed for user {UserId}. Session {SessionId} is no longer valid.", 
                        userId, sessionIdClaim);
                    
                    return Ok(new SessionValidationResponse 
                    { 
                        IsValid = false,
                        Message = "Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador."
                    });
                }

                return Ok(new SessionValidationResponse 
                { 
                    IsValid = true,
                    Message = "Sessão válida"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session");
                return StatusCode(500, new { message = "Erro ao validar sessão" });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? TenantId { get; set; } // Optional - can be resolved from subdomain
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public bool IsSystemOwner { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class TokenValidationRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class TokenValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? TenantId { get; set; }
    }

    public class SessionValidationRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class SessionValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
