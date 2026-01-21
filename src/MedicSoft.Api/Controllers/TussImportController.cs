using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for importing TUSS procedures from official ANS tables
    /// 
    /// Official TUSS table can be downloaded from ANS:
    /// https://www.gov.br/ans/pt-br/assuntos/prestadores/banco-de-dados-de-procedimentos-tuss
    /// </summary>
    [ApiController]
    [Route("api/tuss-import")]
    [Authorize]
    public class TussImportController : BaseController
    {
        private readonly ITussImportService _tussImportService;
        private readonly ILogger<TussImportController> _logger;

        public TussImportController(
            ITussImportService tussImportService,
            ITenantContext tenantContext,
            ILogger<TussImportController> logger) : base(tenantContext)
        {
            _tussImportService = tussImportService;
            _logger = logger;
        }

        /// <summary>
        /// Import TUSS procedures from CSV file
        /// </summary>
        /// <remarks>
        /// Expected CSV format:
        /// ```
        /// Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
        /// 10101012,Consulta médica,01,Consulta médica em consultório,100.00,false
        /// 10101020,Consulta em pronto-socorro,01,Consulta médica em pronto-socorro,150.00,true
        /// ```
        /// 
        /// - Code: 8-digit TUSS code (required)
        /// - Name: Short procedure name (required)
        /// - Category: Category code or name (required)
        /// - Description: Full description (optional, defaults to Name)
        /// - ReferencePrice: Decimal price (required, use dot or comma)
        /// - RequiresAuthorization: true/false or 1/0 (optional, defaults to false)
        /// 
        /// The import will:
        /// - Create new procedures
        /// - Update existing procedures with the same code
        /// - Skip invalid records and report errors
        /// - Process in batches for better performance
        /// </remarks>
        [HttpPost("csv")]
        [RequirePermissionKey(PermissionKeys.TussCreate)]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB limit
        [ProducesResponseType(typeof(ImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImportResult>> ImportFromCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded or file is empty" });
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "File must be a CSV file (.csv extension)" });
                }

                _logger.LogInformation("Starting TUSS CSV import. File: {FileName}, Size: {Size} bytes", 
                    file.FileName, file.Length);

                using var stream = file.OpenReadStream();
                var result = await _tussImportService.ImportFromCsvAsync(stream, GetTenantId());

                if (result.Success)
                {
                    _logger.LogInformation("TUSS import completed successfully: {Summary}", result.GetSummary());
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("TUSS import failed: {ErrorCount} errors", result.Errors.Count);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TUSS CSV import");
                return StatusCode(500, new { message = "Internal server error during import", details = ex.Message });
            }
        }

        /// <summary>
        /// Import TUSS procedures from Excel file
        /// </summary>
        /// <remarks>
        /// Excel import is currently not implemented. Please convert your Excel file to CSV format and use the CSV import endpoint.
        /// 
        /// Expected Excel format (same as CSV):
        /// - First row: Headers (Code, Name, Category, Description, ReferencePrice, RequiresAuthorization)
        /// - Subsequent rows: Data
        /// 
        /// To enable Excel support in the future, install EPPlus or ClosedXML NuGet package.
        /// </remarks>
        [HttpPost("excel")]
        [RequirePermissionKey(PermissionKeys.TussCreate)]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB limit
        [ProducesResponseType(typeof(ImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<ActionResult<ImportResult>> ImportFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded or file is empty" });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return BadRequest(new { message = "File must be an Excel file (.xlsx or .xls extension)" });
                }

                _logger.LogInformation("Starting TUSS Excel import. File: {FileName}, Size: {Size} bytes", 
                    file.FileName, file.Length);

                using var stream = file.OpenReadStream();
                var result = await _tussImportService.ImportFromExcelAsync(stream, GetTenantId());

                // Currently returns 501 Not Implemented
                return StatusCode(501, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TUSS Excel import");
                return StatusCode(500, new { message = "Internal server error during import", details = ex.Message });
            }
        }

        /// <summary>
        /// Get the status of the last TUSS import for the current tenant
        /// </summary>
        [HttpGet("status")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        [ProducesResponseType(typeof(ImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImportResult>> GetLastImportStatus()
        {
            try
            {
                var result = await _tussImportService.GetLastImportStatusAsync(GetTenantId());
                
                if (result == null)
                {
                    return NotFound(new { message = "No import history found for this tenant" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last import status");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Get information about the TUSS import format and official download location
        /// </summary>
        [HttpGet("info")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TussImportInfo), StatusCodes.Status200OK)]
        public ActionResult<TussImportInfo> GetImportInfo()
        {
            var info = new TussImportInfo
            {
                OfficialSource = "ANS - Agência Nacional de Saúde Suplementar",
                DownloadUrl = "https://www.gov.br/ans/pt-br/assuntos/prestadores/banco-de-dados-de-procedimentos-tuss",
                ExpectedFormat = "CSV",
                RequiredColumns = new[] { "Code", "Name", "Category", "ReferencePrice" },
                OptionalColumns = new[] { "Description", "RequiresAuthorization" },
                SampleCsv = @"Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
10101012,Consulta médica,01,Consulta médica em consultório,100.00,false
10101020,Consulta em pronto-socorro,01,Consulta médica em pronto-socorro,150.00,true
20101015,Raio X de tórax,02,Radiografia de tórax PA e perfil,80.00,false",
                Notes = new[]
                {
                    "Code must be exactly 8 digits",
                    "ReferencePrice can use comma or dot as decimal separator",
                    "RequiresAuthorization accepts: true/false, yes/no, 1/0, sim/não",
                    "If Description is empty, Name will be used",
                    "Existing procedures with same code will be updated",
                    "Invalid records will be skipped with error details in the result"
                }
            };

            return Ok(info);
        }
    }

    /// <summary>
    /// Information about TUSS import format
    /// </summary>
    public class TussImportInfo
    {
        public string OfficialSource { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ExpectedFormat { get; set; } = string.Empty;
        public string[] RequiredColumns { get; set; } = Array.Empty<string>();
        public string[] OptionalColumns { get; set; } = Array.Empty<string>();
        public string SampleCsv { get; set; } = string.Empty;
        public string[] Notes { get; set; } = Array.Empty<string>();
    }
}
