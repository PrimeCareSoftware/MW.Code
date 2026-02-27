using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Interfaces;

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
        private readonly IClinicSelectionService _clinicSelectionService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IUserRepository _userRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService, 
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger,
            IClinicSelectionService clinicSelectionService,
            ITwoFactorAuthService twoFactorAuthService,
            IUserRepository userRepository,
            IOwnerRepository ownerRepository,
            IConfiguration configuration)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _clinicSelectionService = clinicSelectionService;
            _twoFactorAuthService = twoFactorAuthService;
            _userRepository = userRepository;
            _ownerRepository = ownerRepository;
            _configuration = configuration;
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

                // Check if user must change their password before proceeding
                if (user.MustChangePassword)
                {
                    _logger.LogInformation("User {UserId} must change password on next login", user.Id);
                    var changePasswordTempToken = GeneratePasswordChangeTempToken(user.Id.ToString(), tenantId, "user");
                    return Ok(new LoginResponse
                    {
                        RequiresPasswordChange = true,
                        TempToken = changePasswordTempToken,
                        Username = user.Username,
                        TenantId = tenantId
                    });
                }

                // Get available clinics for the user
                var availableClinics = await _clinicSelectionService.GetUserClinicsAsync(user.Id, tenantId);
                var clinicList = availableClinics.ToList();

                // Set current clinic if not already set
                Guid? currentClinicId = user.CurrentClinicId;
                if (!currentClinicId.HasValue && clinicList.Any())
                {
                    var preferredClinic = clinicList.FirstOrDefault(c => c.IsPreferred) ?? clinicList.FirstOrDefault();
                    if (preferredClinic == null)
                        return BadRequest(new { message = "No available clinics for user" });
                        
                    var switchResult = await _clinicSelectionService.SwitchClinicAsync(user.Id, preferredClinic.ClinicId, tenantId);
                    if (switchResult.Success)
                    {
                        currentClinicId = switchResult.CurrentClinicId;
                    }
                }

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

                // Check MFA status and method
                var mfaEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(user.Id.ToString(), tenantId);
                var requiresMfaSetup = user.MfaRequiredByPolicy && !mfaEnabled;

                // If MFA is enabled, check the method
                if (mfaEnabled)
                {
                    var mfaMethod = await _twoFactorAuthService.GetTwoFactorMethodAsync(user.Id.ToString(), tenantId);
                    
                    // If email-based 2FA, send verification code
                    if (mfaMethod == MedicSoft.Domain.Enums.TwoFactorMethod.Email)
                    {
                        try
                        {
                            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                            var tempToken = await _twoFactorAuthService.GenerateAndSendEmailCodeAsync(
                                user.Id.ToString(), 
                                user.Email, 
                                ipAddress, 
                                "Login", 
                                tenantId
                            );

                            _logger.LogInformation("2FA email code sent to user: {UserId}", user.Id);

                            return Ok(new TwoFactorRequiredResponse
                            {
                                RequiresTwoFactor = true,
                                TempToken = tempToken,
                                Method = "Email",
                                Message = "Código de verificação enviado para seu e-mail"
                            });
                        }
                        catch (InvalidOperationException ex)
                        {
                            _logger.LogWarning(ex, "Rate limit exceeded for user: {UserId}", user.Id);
                            return StatusCode(429, new { message = ex.Message });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send 2FA code to user: {UserId}", user.Id);
                            return StatusCode(500, new { message = "Erro ao enviar código de verificação. Por favor, tente novamente." });
                        }
                    }
                    
                    // For TOTP-based 2FA, the client should call the MFA verify endpoint
                    // Return token but indicate MFA verification is required
                    if (mfaMethod == MedicSoft.Domain.Enums.TwoFactorMethod.TOTP)
                    {
                        return Ok(new TwoFactorRequiredResponse
                        {
                            RequiresTwoFactor = true,
                            TempToken = token,
                            Method = "TOTP",
                            Message = "Verificação de dois fatores necessária. Use seu aplicativo autenticador."
                        });
                    }
                }

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    TenantId = tenantId,
                    Role = user.Role.ToString(),
                    ClinicId = user.ClinicId,
                    CurrentClinicId = currentClinicId ?? user.ClinicId,
                    AvailableClinics = clinicList,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should match JWT expiry
                    MfaEnabled = mfaEnabled,
                    RequiresMfaSetup = requiresMfaSetup,
                    MfaGracePeriodEndsAt = user.MfaGracePeriodEndsAt
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

                // Check if email is confirmed
                if (!owner.IsEmailConfirmed)
                {
                    _logger.LogWarning("Owner login blocked - email not confirmed for owner: {OwnerId}", owner.Id);
                    return Unauthorized(new 
                    { 
                        message = "Você precisa confirmar seu e-mail antes de fazer login. Verifique sua caixa de entrada e clique no link de confirmação que foi enviado.",
                        code = "EMAIL_NOT_CONFIRMED"
                    });
                }

                _logger.LogInformation("Owner authenticated successfully: {OwnerId}, username: {Username}", 
                    owner.Id, owner.Username);

                // Check if owner must change their password before proceeding
                if (owner.MustChangePassword)
                {
                    _logger.LogInformation("Owner {OwnerId} must change password on next login", owner.Id);
                    var changePasswordTempToken = GeneratePasswordChangeTempToken(owner.Id.ToString(), tenantId, "owner");
                    return Ok(new LoginResponse
                    {
                        RequiresPasswordChange = true,
                        TempToken = changePasswordTempToken,
                        Username = owner.Username,
                        TenantId = tenantId
                    });
                }

                // Check if owner has email 2FA enabled
                var ownerMfaEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(owner.Id.ToString(), tenantId);
                if (ownerMfaEnabled)
                {
                    var ownerMfaMethod = await _twoFactorAuthService.GetTwoFactorMethodAsync(owner.Id.ToString(), tenantId);
                    if (ownerMfaMethod == MedicSoft.Domain.Enums.TwoFactorMethod.Email)
                    {
                        try
                        {
                            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                            var tempToken = await _twoFactorAuthService.GenerateAndSendEmailCodeAsync(
                                owner.Id.ToString(),
                                owner.Email,
                                ipAddress,
                                "Login",
                                tenantId
                            );

                            _logger.LogInformation("2FA email code sent to owner: {OwnerId}", owner.Id);

                            return Ok(new TwoFactorRequiredResponse
                            {
                                RequiresTwoFactor = true,
                                TempToken = tempToken,
                                Method = "Email",
                                Message = "Código de verificação enviado para seu e-mail"
                            });
                        }
                        catch (InvalidOperationException ex)
                        {
                            _logger.LogWarning(ex, "Rate limit exceeded for owner: {OwnerId}", owner.Id);
                            return StatusCode(429, new { message = ex.Message });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send 2FA code to owner: {OwnerId}", owner.Id);
                            return StatusCode(500, new { message = "Erro ao enviar código de verificação. Por favor, tente novamente." });
                        }
                    }
                }

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
                // System owners (no clinic) get SystemAdmin role, clinic owners get ClinicOwner role
                // IsSystemOwner is a computed property that returns !ClinicId.HasValue, but we check both for clarity
                var userRole = (owner.IsSystemOwner && !owner.ClinicId.HasValue) ? RoleNames.SystemAdmin : RoleNames.ClinicOwner;
                
                var token = _jwtTokenService.GenerateToken(
                    username: owner.Username,
                    userId: owner.Id.ToString(),
                    tenantId: tenantId,
                    role: userRole,
                    clinicId: owner.ClinicId?.ToString(),
                    isSystemOwner: owner.IsSystemOwner,
                    sessionId: sessionId,
                    ownerId: owner.Id.ToString()
                );

                _logger.LogInformation("JWT token generated successfully for owner: {OwnerId} with role: {Role}", owner.Id, userRole);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = owner.Username,
                    TenantId = tenantId,
                    Role = userRole,
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
                
                // Check if this is an owner (Owner/ClinicOwner/SystemAdmin) or regular user
                // "Owner" is a legacy role name, kept for backward compatibility with existing tokens
                #pragma warning disable CS0618 // Type or member is obsolete
                if (roleClaim == RoleNames.Owner || roleClaim == RoleNames.ClinicOwner || roleClaim == RoleNames.SystemAdmin)
                #pragma warning restore CS0618
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

        /// <summary>
        /// Verify 2FA email code and complete login
        /// </summary>
        [HttpPost("verify-2fa-email")]
        public async Task<ActionResult<LoginResponse>> VerifyTwoFactorEmail([FromBody] VerifyTwoFactorEmailRequest request)
        {
            try
            {
                // Decode temp token to get userId and tokenId
                var decodedBytes = Convert.FromBase64String(request.TempToken);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');
                
                if (parts.Length != 2)
                {
                    return BadRequest(new { message = "Token temporário inválido" });
                }

                var userId = parts[0];
                var tokenId = parts[1];

                // Get tenantId from context
                var tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    return BadRequest(new { message = "Identificador da clínica não encontrado" });
                }

                // Verify the code
                var isValid = await _twoFactorAuthService.VerifyEmailCodeAsync(userId, request.Code, tenantId);
                if (!isValid)
                {
                    _logger.LogWarning("Failed 2FA email verification for user: {UserId}", userId);
                    return BadRequest(new { message = "Código inválido ou expirado" });
                }

                // Get user
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return BadRequest(new { message = "ID de usuário inválido" });
                }

                var user = await _userRepository.GetByIdAsync(userGuid, tenantId);
                if (user == null)
                {
                    return BadRequest(new { message = "Usuário não encontrado" });
                }

                // Get available clinics
                var availableClinics = await _clinicSelectionService.GetUserClinicsAsync(user.Id, tenantId);
                var clinicList = availableClinics.ToList();

                // Record login
                var sessionId = await _authService.RecordUserLoginAsync(user.Id, tenantId);

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(
                    username: user.Username,
                    userId: user.Id.ToString(),
                    tenantId: tenantId,
                    role: user.Role.ToString(),
                    clinicId: user.ClinicId?.ToString(),
                    sessionId: sessionId
                );

                _logger.LogInformation("User {UserId} completed 2FA email verification and logged in successfully", user.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    TenantId = tenantId,
                    Role = user.Role.ToString(),
                    ClinicId = user.ClinicId,
                    CurrentClinicId = user.CurrentClinicId ?? user.ClinicId,
                    AvailableClinics = clinicList,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    MfaEnabled = true,
                    RequiresMfaSetup = false,
                    MfaGracePeriodEndsAt = user.MfaGracePeriodEndsAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA email code");
                return StatusCode(500, new { message = "Erro ao verificar código de verificação" });
            }
        }

        /// <summary>
        /// Resend 2FA email code
        /// </summary>
        [HttpPost("resend-2fa-email")]
        public async Task<ActionResult> ResendTwoFactorEmail([FromBody] ResendTwoFactorEmailRequest request)
        {
            try
            {
                // Decode temp token to get userId
                var decodedBytes = Convert.FromBase64String(request.TempToken);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');
                
                if (parts.Length != 2)
                {
                    return BadRequest(new { message = "Token temporário inválido" });
                }

                var userId = parts[0];

                // Get tenantId from context
                var tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    return BadRequest(new { message = "Identificador da clínica não encontrado" });
                }

                // Get user to get email
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return BadRequest(new { message = "ID de usuário inválido" });
                }

                var user = await _userRepository.GetByIdAsync(userGuid, tenantId);
                if (user == null)
                {
                    return BadRequest(new { message = "Usuário não encontrado" });
                }

                // Send new code
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var newTempToken = await _twoFactorAuthService.GenerateAndSendEmailCodeAsync(
                    userId, 
                    user.Email, 
                    ipAddress, 
                    "Login", 
                    tenantId
                );

                _logger.LogInformation("2FA email code resent to user: {UserId}", userId);

                return Ok(new { 
                    message = "Código reenviado com sucesso",
                    tempToken = newTempToken
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Rate limit exceeded when resending 2FA code");
                return StatusCode(429, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending 2FA email code");
                return StatusCode(500, new { message = "Erro ao reenviar código de verificação" });
            }
        }

        /// <summary>
        /// Verify owner 2FA email code and complete login
        /// </summary>
        [HttpPost("owner-verify-2fa-email")]
        public async Task<ActionResult<LoginResponse>> OwnerVerifyTwoFactorEmail([FromBody] VerifyTwoFactorEmailRequest request)
        {
            try
            {
                // Decode temp token to get ownerId and tokenId
                var decodedBytes = Convert.FromBase64String(request.TempToken);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');

                if (parts.Length != 2)
                {
                    return BadRequest(new { message = "Token temporário inválido" });
                }

                var ownerId = parts[0];
                var tokenId = parts[1];

                // Get tenantId from context
                var tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    return BadRequest(new { message = "Identificador da clínica não encontrado" });
                }

                // Verify the code
                var isValid = await _twoFactorAuthService.VerifyEmailCodeAsync(ownerId, request.Code, tenantId);
                if (!isValid)
                {
                    _logger.LogWarning("Failed 2FA email verification for owner: {OwnerId}", ownerId);
                    return BadRequest(new { message = "Código inválido ou expirado" });
                }

                // Get owner
                if (!Guid.TryParse(ownerId, out var ownerGuid))
                {
                    return BadRequest(new { message = "ID de proprietário inválido" });
                }

                var owner = await _ownerRepository.GetByIdAsync(ownerGuid, tenantId);
                if (owner == null)
                {
                    return BadRequest(new { message = "Proprietário não encontrado" });
                }

                // Record login
                var sessionId = await _authService.RecordOwnerLoginAsync(owner.Id, tenantId);

                // Generate JWT token
                var ownerRole = (owner.IsSystemOwner && !owner.ClinicId.HasValue) ? RoleNames.SystemAdmin : RoleNames.ClinicOwner;
                var token = _jwtTokenService.GenerateToken(
                    username: owner.Username,
                    userId: owner.Id.ToString(),
                    tenantId: tenantId,
                    role: ownerRole,
                    clinicId: owner.ClinicId?.ToString(),
                    isSystemOwner: owner.IsSystemOwner,
                    sessionId: sessionId,
                    ownerId: owner.Id.ToString()
                );

                _logger.LogInformation("Owner {OwnerId} completed 2FA email verification and logged in successfully", owner.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = owner.Username,
                    TenantId = tenantId,
                    Role = ownerRole,
                    ClinicId = owner.ClinicId,
                    IsSystemOwner = owner.IsSystemOwner,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    MfaEnabled = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying owner 2FA email code");
                return StatusCode(500, new { message = "Erro ao verificar código de verificação" });
            }
        }

        /// <summary>
        /// Resend owner 2FA email code
        /// </summary>
        [HttpPost("owner-resend-2fa-email")]
        public async Task<ActionResult> OwnerResendTwoFactorEmail([FromBody] ResendTwoFactorEmailRequest request)
        {
            try
            {
                // Decode temp token to get ownerId
                var decodedBytes = Convert.FromBase64String(request.TempToken);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');

                if (parts.Length != 2)
                {
                    return BadRequest(new { message = "Token temporário inválido" });
                }

                var ownerId = parts[0];

                // Get tenantId from context
                var tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    return BadRequest(new { message = "Identificador da clínica não encontrado" });
                }

                if (!Guid.TryParse(ownerId, out var ownerGuid))
                {
                    return BadRequest(new { message = "ID de proprietário inválido" });
                }

                var owner = await _ownerRepository.GetByIdAsync(ownerGuid, tenantId);
                if (owner == null)
                {
                    return BadRequest(new { message = "Proprietário não encontrado" });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var newTempToken = await _twoFactorAuthService.GenerateAndSendEmailCodeAsync(
                    ownerId,
                    owner.Email,
                    ipAddress,
                    "Login",
                    tenantId
                );

                _logger.LogInformation("2FA email code resent to owner: {OwnerId}", ownerId);

                return Ok(new {
                    message = "Código reenviado com sucesso",
                    tempToken = newTempToken
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Rate limit exceeded when resending owner 2FA code");
                return StatusCode(429, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending owner 2FA email code");
                return StatusCode(500, new { message = "Erro ao reenviar código de verificação" });
            }
        }

        /// <summary>
        /// Complete the required password change during first login
        /// </summary>
        [HttpPost("complete-password-change")]
        public async Task<ActionResult<LoginResponse>> CompletePasswordChange([FromBody] CompletePasswordChangeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.TempToken) ||
                    string.IsNullOrWhiteSpace(request.NewPassword) ||
                    string.IsNullOrWhiteSpace(request.ConfirmPassword))
                {
                    return BadRequest(new { message = "Todos os campos são obrigatórios." });
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest(new { message = "As senhas não conferem." });
                }

                if (request.NewPassword.Length < 8)
                {
                    return BadRequest(new { message = "A senha deve ter pelo menos 8 caracteres." });
                }

                // Validate temp token using HMAC-signed token
                var (entityId, tenantId, entityType, tokenIsValid) = ValidatePasswordChangeTempToken(request.TempToken);
                if (!tokenIsValid)
                {
                    return BadRequest(new { message = "Token temporário inválido ou expirado." });
                }

                if (!Guid.TryParse(entityId, out var entityGuid))
                {
                    return BadRequest(new { message = "Token temporário inválido." });
                }

                if (entityType == "user")
                {
                    var user = await _userRepository.GetByIdAsync(entityGuid, tenantId);
                    if (user == null || !user.MustChangePassword)
                    {
                        return BadRequest(new { message = "Solicitação de troca de senha inválida." });
                    }

                    var changed = await _authService.ChangeUserPasswordAsync(entityGuid, request.NewPassword, tenantId);
                    if (!changed)
                    {
                        return StatusCode(500, new { message = "Erro ao alterar a senha." });
                    }

                    // Reload user after password change
                    user = await _userRepository.GetByIdAsync(entityGuid, tenantId);
                    if (user == null)
                        return StatusCode(500, new { message = "Erro ao processar login." });

                    // Get available clinics
                    var availableClinics = await _clinicSelectionService.GetUserClinicsAsync(user.Id, tenantId);
                    var clinicList = availableClinics.ToList();

                    // Record login
                    var sessionId = await _authService.RecordUserLoginAsync(user.Id, tenantId);

                    var token = _jwtTokenService.GenerateToken(
                        username: user.Username,
                        userId: user.Id.ToString(),
                        tenantId: tenantId,
                        role: user.Role.ToString(),
                        clinicId: user.ClinicId?.ToString(),
                        sessionId: sessionId
                    );

                    _logger.LogInformation("User {UserId} completed required password change", user.Id);

                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Username = user.Username,
                        TenantId = tenantId,
                        Role = user.Role.ToString(),
                        ClinicId = user.ClinicId,
                        CurrentClinicId = user.CurrentClinicId ?? user.ClinicId,
                        AvailableClinics = clinicList,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                    });
                }
                else if (entityType == "owner")
                {
                    var owner = await _ownerRepository.GetByIdAsync(entityGuid, tenantId);
                    if (owner == null || !owner.MustChangePassword)
                    {
                        return BadRequest(new { message = "Solicitação de troca de senha inválida." });
                    }

                    var changed = await _authService.ChangeOwnerPasswordAsync(entityGuid, request.NewPassword, tenantId);
                    if (!changed)
                    {
                        return StatusCode(500, new { message = "Erro ao alterar a senha." });
                    }

                    // Reload owner after password change
                    owner = await _ownerRepository.GetByIdAsync(entityGuid, tenantId);
                    if (owner == null)
                        return StatusCode(500, new { message = "Erro ao processar login." });

                    // Record login
                    var sessionId = await _authService.RecordOwnerLoginAsync(owner.Id, tenantId);

                    var ownerRole = (owner.IsSystemOwner && !owner.ClinicId.HasValue) ? RoleNames.SystemAdmin : RoleNames.ClinicOwner;
                    var token = _jwtTokenService.GenerateToken(
                        username: owner.Username,
                        userId: owner.Id.ToString(),
                        tenantId: tenantId,
                        role: ownerRole,
                        clinicId: owner.ClinicId?.ToString(),
                        isSystemOwner: owner.IsSystemOwner,
                        sessionId: sessionId,
                        ownerId: owner.Id.ToString()
                    );

                    _logger.LogInformation("Owner {OwnerId} completed required password change", owner.Id);

                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Username = owner.Username,
                        TenantId = tenantId,
                        Role = ownerRole,
                        ClinicId = owner.ClinicId,
                        IsSystemOwner = owner.IsSystemOwner,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                    });
                }
                else
                {
                    return BadRequest(new { message = "Token temporário inválido." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing required password change");
                return StatusCode(500, new { message = "Erro ao alterar a senha. Por favor, tente novamente." });
            }
        }

        /// <summary>
        /// Register the first system owner (organization owner) - PRODUCTION ONLY
        /// This endpoint is secured with a special registration token that must be set in environment variables.
        /// Use this endpoint once to create the initial system administrator account.
        /// After creating the account, remove or rotate the SYSTEM_OWNER_REGISTRATION_TOKEN for security.
        /// </summary>
        [HttpPost("register-system-owner")]
        public async Task<ActionResult> RegisterSystemOwner([FromBody] RegisterSystemOwnerRequest request)
        {
            try
            {
                // Get the registration token from configuration
                var validRegistrationToken = _configuration["SYSTEM_OWNER_REGISTRATION_TOKEN"];
                
                if (string.IsNullOrWhiteSpace(validRegistrationToken))
                {
                    _logger.LogError("System owner registration attempted but SYSTEM_OWNER_REGISTRATION_TOKEN not configured");
                    return StatusCode(503, new { message = "System owner registration is not configured. Contact support." });
                }

                // Validate the registration token
                if (request.RegistrationToken != validRegistrationToken)
                {
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    _logger.LogWarning("Invalid system owner registration token provided. IP: {IpAddress}", ipAddress);
                    return StatusCode(403, new { message = "Token de registro inválido." });
                }

                // Validate request
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.FullName) ||
                    string.IsNullOrWhiteSpace(request.TenantId))
                {
                    return BadRequest(new { message = "Todos os campos são obrigatórios." });
                }

                // Check if a system owner already exists (security check)
                var existingSystemOwner = await _authService.GetSystemOwnerAsync(request.TenantId);
                if (existingSystemOwner != null)
                {
                    _logger.LogWarning("Attempt to create system owner when one already exists. TenantId: {TenantId}", request.TenantId);
                    return BadRequest(new { message = "Um administrador do sistema já existe. Use o endpoint de gerenciamento de owners para criar owners adicionais." });
                }

                // Create the system owner (ClinicId = null makes it a system owner)
                var owner = await _authService.CreateSystemOwnerAsync(
                    request.Username,
                    request.Password,
                    request.Email,
                    request.FullName,
                    request.Phone ?? "",
                    request.TenantId
                );

                _logger.LogInformation("System owner created successfully. OwnerId: {OwnerId}, Username: {Username}", 
                    owner.Id, owner.Username);

                return Ok(new { 
                    message = "Administrador do sistema criado com sucesso. Por favor, faça login.",
                    ownerId = owner.Id,
                    username = owner.Username
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "System owner registration failed due to validation: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during system owner registration");
                return StatusCode(500, new { message = "Erro ao criar administrador do sistema. Por favor, tente novamente." });
            }
        }

        private string GeneratePasswordChangeTempToken(string entityId, string tenantId, string entityType)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "fallback-secret-key";
            var expiryUnixTs = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
            var payload = $"{entityId}:{tenantId}:{entityType}:password_change:{expiryUnixTs}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}:{signature}"));
        }

        private (string entityId, string tenantId, string entityType, bool isValid) ValidatePasswordChangeTempToken(string tempToken)
        {
            try
            {
                var secretKey = _configuration["JwtSettings:SecretKey"] ?? "fallback-secret-key";
                var decodedBytes = Convert.FromBase64String(tempToken);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                // Format: {entityId}:{tenantId}:{entityType}:password_change:{expiryUnixTs}:{signature}
                var lastColon = decodedString.LastIndexOf(':');
                if (lastColon < 0) return (string.Empty, string.Empty, string.Empty, false);

                var signature = decodedString[(lastColon + 1)..];
                var payload = decodedString[..lastColon];

                // Verify HMAC signature
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                var expectedSignature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(signature),
                        Encoding.UTF8.GetBytes(expectedSignature)))
                {
                    return (string.Empty, string.Empty, string.Empty, false);
                }

                // Parse payload parts: {entityId}:{tenantId}:{entityType}:password_change:{expiryUnixTs}
                var parts = payload.Split(':');
                if (parts.Length != 5 || parts[3] != "password_change")
                    return (string.Empty, string.Empty, string.Empty, false);

                // Validate expiry
                if (!long.TryParse(parts[4], out var expiryUnixTs))
                    return (string.Empty, string.Empty, string.Empty, false);

                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiryUnixTs)
                    return (string.Empty, string.Empty, string.Empty, false);

                return (parts[0], parts[1], parts[2], true);
            }
            catch
            {
                return (string.Empty, string.Empty, string.Empty, false);
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
        public Guid? CurrentClinicId { get; set; } // The clinic currently selected by the user
        public bool IsSystemOwner { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<UserClinicDto>? AvailableClinics { get; set; } // List of clinics user can access
        
        // MFA-related properties
        public bool MfaEnabled { get; set; }
        public bool RequiresMfaSetup { get; set; }
        public DateTime? MfaGracePeriodEndsAt { get; set; }
        
        // Password change required
        public bool RequiresPasswordChange { get; set; }
        public string? TempToken { get; set; }
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

    public class TwoFactorRequiredResponse
    {
        public bool RequiresTwoFactor { get; set; } = true;
        public string TempToken { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class VerifyTwoFactorEmailRequest
    {
        public string TempToken { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ResendTwoFactorEmailRequest
    {
        public string TempToken { get; set; } = string.Empty;
    }

    public class RegisterSystemOwnerRequest
    {
        public string RegistrationToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }

    public class CompletePasswordChangeRequest
    {
        public string TempToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
