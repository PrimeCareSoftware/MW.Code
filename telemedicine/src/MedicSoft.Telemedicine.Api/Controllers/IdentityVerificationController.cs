using Microsoft.AspNetCore.Mvc;
using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Interfaces;

namespace MedicSoft.Telemedicine.Api.Controllers;

/// <summary>
/// Identity verification management endpoints
/// Required for CFM 2.314/2022 compliance (bidirectional identification)
/// </summary>
[ApiController]
[Route("api/telemedicine/[controller]")]
[Produces("application/json")]
public class IdentityVerificationController : ControllerBase
{
    private readonly ITelemedicineService _telemedicineService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICfmValidationService _cfmValidationService;
    private readonly ILogger<IdentityVerificationController> _logger;

    public IdentityVerificationController(
        ITelemedicineService telemedicineService,
        IFileStorageService fileStorageService,
        ICfmValidationService cfmValidationService,
        ILogger<IdentityVerificationController> logger)
    {
        _telemedicineService = telemedicineService ?? throw new ArgumentNullException(nameof(telemedicineService));
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _cfmValidationService = cfmValidationService ?? throw new ArgumentNullException(nameof(cfmValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates identity verification with document upload
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityVerificationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IdentityVerificationResponse>> CreateVerification(
        [FromForm] CreateIdentityVerificationRequest request,
        [FromForm] IFormFile documentPhoto,
        [FromForm] IFormFile? selfie,
        [FromForm] IFormFile? crmCardPhoto,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (documentPhoto == null || documentPhoto.Length == 0)
                return BadRequest("Document photo is required");

            // Validate provider requirements
            if (request.UserType == "Provider")
            {
                if (crmCardPhoto == null || crmCardPhoto.Length == 0)
                    return BadRequest("CRM card photo is required for providers");
                    
                if (string.IsNullOrWhiteSpace(request.CrmNumber))
                    return BadRequest("CRM number is required for providers");
                    
                if (string.IsNullOrWhiteSpace(request.CrmState))
                    return BadRequest("CRM state is required for providers");
            }

            // Save files to secure encrypted storage
            var containerName = $"identity-documents-{tenantId}";
            var documentPhotoPath = await _fileStorageService.SaveFileAsync(
                documentPhoto, 
                containerName, 
                $"document_{request.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(documentPhoto.FileName)}",
                encrypt: true);

            string? selfiePath = null;
            if (selfie != null)
            {
                selfiePath = await _fileStorageService.SaveFileAsync(
                    selfie,
                    containerName,
                    $"selfie_{request.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(selfie.FileName)}",
                    encrypt: true);
            }

            string? crmCardPhotoPath = null;
            if (crmCardPhoto != null)
            {
                crmCardPhotoPath = await _fileStorageService.SaveFileAsync(
                    crmCardPhoto,
                    containerName,
                    $"crm_{request.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(crmCardPhoto.FileName)}",
                    encrypt: true);
            }

            var result = await _telemedicineService.CreateIdentityVerificationAsync(
                request,
                documentPhotoPath,
                selfiePath,
                crmCardPhotoPath,
                tenantId);

            return CreatedAtAction(nameof(GetVerificationById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while creating identity verification");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating identity verification");
            return StatusCode(500, "An error occurred while creating identity verification");
        }
    }

    /// <summary>
    /// Gets identity verification by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IdentityVerificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IdentityVerificationResponse>> GetVerificationById(
        Guid id,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetIdentityVerificationByIdAsync(id, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting identity verification {VerificationId}", id);
            return StatusCode(500, "An error occurred while retrieving identity verification");
        }
    }

    /// <summary>
    /// Gets latest identity verification for a user
    /// </summary>
    [HttpGet("user/{userId}/latest")]
    [ProducesResponseType(typeof(IdentityVerificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IdentityVerificationResponse>> GetLatestVerification(
        Guid userId,
        [FromQuery] string userType,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (string.IsNullOrWhiteSpace(userType))
                return BadRequest("User type is required");

            var result = await _telemedicineService.GetLatestIdentityVerificationAsync(userId, userType, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest verification for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving identity verification");
        }
    }

    /// <summary>
    /// Checks if user has valid identity verification
    /// </summary>
    [HttpGet("user/{userId}/is-valid")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> HasValidVerification(
        Guid userId,
        [FromQuery] string userType,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (string.IsNullOrWhiteSpace(userType))
                return BadRequest("User type is required");

            var result = await _telemedicineService.HasValidIdentityVerificationAsync(userId, userType, tenantId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking verification for user {UserId}", userId);
            return StatusCode(500, "An error occurred while checking identity verification");
        }
    }

    /// <summary>
    /// Gets pending identity verifications
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<IdentityVerificationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IdentityVerificationResponse>>> GetPendingVerifications(
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetPendingVerificationsAsync(tenantId, skip, take);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending verifications");
            return StatusCode(500, "An error occurred while retrieving pending verifications");
        }
    }

    /// <summary>
    /// Approves or rejects identity verification
    /// </summary>
    [HttpPost("{id}/verify")]
    [ProducesResponseType(typeof(IdentityVerificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IdentityVerificationResponse>> VerifyIdentity(
        Guid id,
        [FromBody] VerifyIdentityRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromHeader(Name = "X-User-Id")] Guid verifiedByUserId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (verifiedByUserId == Guid.Empty)
                return BadRequest("User ID header is required");

            var result = await _telemedicineService.VerifyIdentityAsync(id, request, verifiedByUserId, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while verifying identity {VerificationId}", id);
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while verifying identity");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying identity {VerificationId}", id);
            return StatusCode(500, "An error occurred while verifying identity");
        }
    }

    /// <summary>
    /// Validates CRM with CFM API before creating identity verification
    /// </summary>
    [HttpPost("validate-crm")]
    [ProducesResponseType(typeof(CfmCrmValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CfmCrmValidationResult>> ValidateCrmWithCfm(
        [FromBody] ValidateCrmRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CrmNumber))
                return BadRequest("CRM number is required");

            if (string.IsNullOrWhiteSpace(request.CrmState))
                return BadRequest("CRM state is required");

            var result = await _cfmValidationService.ValidateCrmAsync(request.CrmNumber, request.CrmState);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CRM with CFM");
            return StatusCode(500, "An error occurred while validating CRM");
        }
    }

    /// <summary>
    /// Validates CPF with CFM API
    /// </summary>
    [HttpPost("validate-cpf")]
    [ProducesResponseType(typeof(CfmCpfValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CfmCpfValidationResult>> ValidateCpfWithCfm(
        [FromBody] ValidateCpfRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Cpf))
                return BadRequest("CPF is required");

            var result = await _cfmValidationService.ValidateCpfAsync(request.Cpf);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CPF with CFM");
            return StatusCode(500, "An error occurred while validating CPF");
        }
    }
}

public class ValidateCrmRequest
{
    public string CrmNumber { get; set; } = string.Empty;
    public string CrmState { get; set; } = string.Empty;
}

public class ValidateCpfRequest
{
    public string Cpf { get; set; } = string.Empty;
    }
}
