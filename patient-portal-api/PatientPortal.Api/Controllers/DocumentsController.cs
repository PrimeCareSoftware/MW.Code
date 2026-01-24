using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Application.DTOs.Documents;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : BaseController
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all medical documents for the authenticated patient with pagination
    /// </summary>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 50, max: 100)</param>
    /// <returns>List of medical documents with metadata</returns>
    /// <response code="200">Returns the list of documents</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents?skip=0&amp;take=20
    ///     Authorization: Bearer {access-token}
    /// 
    /// Returns all medical documents (prescriptions, reports, lab results, etc.)
    /// associated with the authenticated patient.
    /// 
    /// **Document Metadata Includes:**
    /// - Document ID and title
    /// - Document type (Prescription, MedicalReport, LabResult, etc.)
    /// - Issue date and issuing doctor
    /// - File format and size
    /// - Associated appointment (if applicable)
    /// 
    /// Use the document ID with the download endpoint to retrieve the actual file.
    /// </remarks>
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
    /// Retrieves recently issued documents for the authenticated patient
    /// </summary>
    /// <param name="take">Maximum number of recent documents to return (default: 10)</param>
    /// <returns>List of most recent documents sorted by issue date</returns>
    /// <response code="200">Returns the list of recent documents</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents/recent?take=5
    ///     Authorization: Bearer {access-token}
    /// 
    /// Returns the most recently issued documents sorted by issue date in descending order.
    /// Useful for dashboard widgets showing latest documents.
    /// </remarks>
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
    /// Retrieves a specific document by its ID
    /// </summary>
    /// <param name="id">Unique identifier of the document</param>
    /// <returns>Detailed metadata about the document</returns>
    /// <response code="200">Returns the document metadata</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Document not found or doesn't belong to the authenticated patient</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     Authorization: Bearer {access-token}
    /// 
    /// Returns document metadata only. To download the actual file,
    /// use the /api/documents/{id}/download endpoint.
    /// 
    /// Security: Users can only access their own documents.
    /// </remarks>
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
    /// Retrieves documents filtered by type with pagination
    /// </summary>
    /// <param name="type">Document type to filter by (Prescription=0, MedicalReport=1, LabResult=2, Imaging=3, MedicalCertificate=4, Referral=5, Other=6)</param>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 50, max: 100)</param>
    /// <returns>List of documents matching the specified type</returns>
    /// <response code="200">Returns the filtered list of documents</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents/type/0?skip=0&amp;take=20
    ///     Authorization: Bearer {access-token}
    /// 
    /// **Document Type Values:**
    /// - 0 = Prescription (receitas médicas)
    /// - 1 = MedicalReport (laudos e relatórios médicos)
    /// - 2 = LabResult (resultados de exames laboratoriais)
    /// - 3 = Imaging (exames de imagem - raio-x, tomografia, etc.)
    /// - 4 = MedicalCertificate (atestados médicos)
    /// - 5 = Referral (encaminhamentos para especialistas)
    /// - 6 = Other (outros documentos médicos)
    /// 
    /// Useful for filtering documents by category.
    /// </remarks>
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
    /// Gets the total count of documents for the authenticated patient
    /// </summary>
    /// <returns>Total number of documents</returns>
    /// <response code="200">Returns the count of documents</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents/count
    ///     Authorization: Bearer {access-token}
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "count": 15
    ///     }
    /// 
    /// Returns the total count of all medical documents for the authenticated patient.
    /// Useful for dashboard statistics and pagination calculations.
    /// </remarks>
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
    /// Downloads the actual file content of a medical document
    /// </summary>
    /// <param name="id">Unique identifier of the document to download</param>
    /// <returns>File content as binary data</returns>
    /// <response code="200">Returns the document file (typically PDF)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Document not found, doesn't belong to patient, or file is not available</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/documents/3fa85f64-5717-4562-b3fc-2c963f66afa6/download
    ///     Authorization: Bearer {access-token}
    /// 
    /// **Response:**
    /// - Content-Type: application/pdf (or appropriate MIME type)
    /// - Content-Disposition: attachment; filename="document-{id}.pdf"
    /// - Binary file content
    /// 
    /// **Security:**
    /// - Users can only download their own documents
    /// - Download events are logged for audit/compliance (LGPD)
    /// - Files are served from secure storage (Azure Blob/S3)
    /// 
    /// **Note:** Currently returns 404 as file storage integration is pending implementation.
    /// </remarks>
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
}
