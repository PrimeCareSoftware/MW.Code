using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Application.DTOs.Documents;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all documents for the authenticated patient
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DocumentDto>>> GetMyDocuments([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var documents = await _documentService.GetMyDocumentsAsync(userId.Value, skip, take);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents");
            return StatusCode(500, new { message = "An error occurred while retrieving documents" });
        }
    }

    /// <summary>
    /// Get recent documents for the authenticated patient
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<List<DocumentDto>>> GetRecentDocuments([FromQuery] int take = 10)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var documents = await _documentService.GetRecentDocumentsAsync(userId.Value, take);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent documents");
            return StatusCode(500, new { message = "An error occurred while retrieving recent documents" });
        }
    }

    /// <summary>
    /// Get document by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetDocumentById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var document = await _documentService.GetByIdAsync(id, userId.Value);
            
            if (document == null)
                return NotFound(new { message = "Document not found" });

            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document {DocumentId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the document" });
        }
    }

    /// <summary>
    /// Get documents by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<List<DocumentDto>>> GetDocumentsByType(DocumentType type, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var documents = await _documentService.GetByTypeAsync(userId.Value, type, skip, take);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents by type {Type}", type);
            return StatusCode(500, new { message = "An error occurred while retrieving documents" });
        }
    }

    /// <summary>
    /// Get total count of documents
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetDocumentsCount()
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var count = await _documentService.GetCountAsync(userId.Value);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents count");
            return StatusCode(500, new { message = "An error occurred while counting documents" });
        }
    }

    /// <summary>
    /// Download document
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var fileBytes = await _documentService.DownloadDocumentAsync(id, userId.Value);
            
            if (fileBytes == null)
                return NotFound(new { message = "Document not found or not available for download" });

            // TODO: Get actual file name and content type from document metadata
            return File(fileBytes, "application/pdf", $"document-{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", id);
            return StatusCode(500, new { message = "An error occurred while downloading the document" });
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }
}
