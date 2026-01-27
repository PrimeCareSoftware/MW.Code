using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing TISS glosas (rejections/discounts)
    /// </summary>
    [ApiController]
    [Route("api/tiss-glosas")]
    [Authorize]
    public class TissGlosaController : BaseController
    {
        private readonly ITissGlosaService _glosaService;

        public TissGlosaController(
            ITissGlosaService glosaService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _glosaService = glosaService;
        }

        /// <summary>
        /// Get glosa by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissGlosaDto>> GetById(Guid id)
        {
            try
            {
                var glosa = await _glosaService.GetByIdAsync(id, GetTenantId());
                return Ok(glosa);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get glosas by guide ID
        /// </summary>
        [HttpGet("by-guide/{guideId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGlosaDto>>> GetByGuideId(Guid guideId)
        {
            var glosas = await _glosaService.GetByGuideIdAsync(guideId, GetTenantId());
            return Ok(glosas);
        }

        /// <summary>
        /// Get glosas by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGlosaDto>>> GetByStatus(string status)
        {
            if (!Enum.TryParse<StatusGlosa>(status, true, out var statusEnum))
            {
                return BadRequest(new { message = $"Status inválido: {status}" });
            }

            var glosas = await _glosaService.GetByStatusAsync(statusEnum, GetTenantId());
            return Ok(glosas);
        }

        /// <summary>
        /// Get glosas by type
        /// </summary>
        [HttpGet("by-tipo/{tipo}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGlosaDto>>> GetByTipo(string tipo)
        {
            if (!Enum.TryParse<TipoGlosa>(tipo, true, out var tipoEnum))
            {
                return BadRequest(new { message = $"Tipo inválido: {tipo}" });
            }

            var glosas = await _glosaService.GetByTipoAsync(tipoEnum, GetTenantId());
            return Ok(glosas);
        }

        /// <summary>
        /// Get glosas by date range
        /// </summary>
        [HttpGet("by-date-range")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGlosaDto>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var glosas = await _glosaService.GetByDateRangeAsync(start, end, GetTenantId());
            return Ok(glosas);
        }

        /// <summary>
        /// Get glosas pending recursos (appeals)
        /// </summary>
        [HttpGet("pending-recursos")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGlosaDto>>> GetPendingRecursos()
        {
            var glosas = await _glosaService.GetPendingRecursosAsync(GetTenantId());
            return Ok(glosas);
        }

        /// <summary>
        /// Create a new glosa
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGlosaDto>> Create([FromBody] CreateTissGlosaDto dto)
        {
            try
            {
                var glosa = await _glosaService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = glosa.Id },
                    glosa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark glosa as under analysis
        /// </summary>
        [HttpPost("{id}/marcar-em-analise")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGlosaDto>> MarcarEmAnalise(Guid id)
        {
            try
            {
                var glosa = await _glosaService.MarcarEmAnaliseAsync(id, GetTenantId());
                return Ok(glosa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Accept glosa (give up on appeal)
        /// </summary>
        [HttpPost("{id}/acatar")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGlosaDto>> AcatarGlosa(Guid id)
        {
            try
            {
                var glosa = await _glosaService.AcatarGlosaAsync(id, GetTenantId());
                return Ok(glosa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a glosa
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _glosaService.DeleteAsync(id, GetTenantId());
            if (!success)
                return NotFound(new { message = "Glosa não encontrada." });

            return Ok(new { message = "Glosa excluída com sucesso." });
        }
    }
}
