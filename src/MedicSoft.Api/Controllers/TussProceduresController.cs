using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing TUSS procedures
    /// </summary>
    [ApiController]
    [Route("api/tuss-procedures")]
    [Authorize]
    public class TussProceduresController : BaseController
    {
        private readonly ITussProcedureService _tussProcedureService;

        public TussProceduresController(
            ITussProcedureService tussProcedureService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _tussProcedureService = tussProcedureService;
        }

        /// <summary>
        /// Get all TUSS procedures
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<IEnumerable<TussProcedureDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var procedures = await _tussProcedureService.GetAllAsync(GetTenantId(), includeInactive);
            return Ok(procedures);
        }

        /// <summary>
        /// Get TUSS procedure by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<TussProcedureDto>> GetById(Guid id)
        {
            var procedure = await _tussProcedureService.GetByIdAsync(id, GetTenantId());
            if (procedure == null)
                return NotFound(new { message = $"Procedimento TUSS não encontrado." });

            return Ok(procedure);
        }

        /// <summary>
        /// Get TUSS procedure by code
        /// </summary>
        [HttpGet("by-code/{code}")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<TussProcedureDto>> GetByCode(string code)
        {
            var procedure = await _tussProcedureService.GetByCodeAsync(code, GetTenantId());
            if (procedure == null)
                return NotFound(new { message = $"Procedimento com código {code} não encontrado." });

            return Ok(procedure);
        }

        /// <summary>
        /// Search TUSS procedures by code or description
        /// </summary>
        [HttpGet("search")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<IEnumerable<TussProcedureDto>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
                return BadRequest(new { message = "O termo de busca deve ter pelo menos 3 caracteres." });

            var procedures = await _tussProcedureService.SearchAsync(query, GetTenantId());
            return Ok(procedures);
        }

        /// <summary>
        /// Get TUSS procedures by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<IEnumerable<TussProcedureDto>>> GetByCategory(string category)
        {
            var procedures = await _tussProcedureService.GetByCategoryAsync(category, GetTenantId());
            return Ok(procedures);
        }

        /// <summary>
        /// Get procedures that require authorization
        /// </summary>
        [HttpGet("requiring-authorization")]
        [RequirePermissionKey(PermissionKeys.TussView)]
        public async Task<ActionResult<IEnumerable<TussProcedureDto>>> GetRequiringAuthorization()
        {
            var procedures = await _tussProcedureService.GetRequiringAuthorizationAsync(GetTenantId());
            return Ok(procedures);
        }

        /// <summary>
        /// Create a new TUSS procedure
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TussCreate)]
        public async Task<ActionResult<TussProcedureDto>> Create([FromBody] CreateTussProcedureDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var procedure = await _tussProcedureService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = procedure.Id }, procedure);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a TUSS procedure
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.TussEdit)]
        public async Task<ActionResult<TussProcedureDto>> Update(Guid id, [FromBody] UpdateTussProcedureDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var procedure = await _tussProcedureService.UpdateAsync(id, dto, GetTenantId());
                return Ok(procedure);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Import TUSS procedures from CSV file
        /// </summary>
        [HttpPost("import")]
        [RequirePermissionKey(PermissionKeys.TussCreate)]
        public async Task<ActionResult<TussImportResultDto>> Import([FromBody] ImportTussRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
                return BadRequest(new { message = "Caminho do arquivo é obrigatório." });

            try
            {
                var result = await _tussProcedureService.ImportFromCsvAsync(request.FilePath, GetTenantId());
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate a TUSS procedure
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.TussEdit)]
        public async Task<ActionResult<TussProcedureDto>> Activate(Guid id)
        {
            try
            {
                var procedure = await _tussProcedureService.ActivateAsync(id, GetTenantId());
                return Ok(procedure);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a TUSS procedure
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.TussEdit)]
        public async Task<ActionResult<TussProcedureDto>> Deactivate(Guid id)
        {
            try
            {
                var procedure = await _tussProcedureService.DeactivateAsync(id, GetTenantId());
                return Ok(procedure);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a TUSS procedure (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.TussDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _tussProcedureService.DeleteAsync(id, GetTenantId());
                if (!result)
                    return NotFound(new { message = "Procedimento TUSS não encontrado." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ImportTussRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }
}
