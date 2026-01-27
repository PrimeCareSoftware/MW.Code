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
    /// Controller for managing TISS glosa appeals (recursos)
    /// </summary>
    [ApiController]
    [Route("api/tiss-recursos")]
    [Authorize]
    public class TissRecursoController : BaseController
    {
        private readonly ITissRecursoGlosaService _recursoService;

        public TissRecursoController(
            ITissRecursoGlosaService recursoService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _recursoService = recursoService;
        }

        /// <summary>
        /// Get recurso by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissRecursoGlosaDto>> GetById(Guid id)
        {
            try
            {
                var recurso = await _recursoService.GetByIdAsync(id, GetTenantId());
                return Ok(recurso);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get recursos by glosa ID
        /// </summary>
        [HttpGet("by-glosa/{glosaId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissRecursoGlosaDto>>> GetByGlosaId(Guid glosaId)
        {
            var recursos = await _recursoService.GetByGlosaIdAsync(glosaId, GetTenantId());
            return Ok(recursos);
        }

        /// <summary>
        /// Get recursos pending response from operator
        /// </summary>
        [HttpGet("pending-response")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissRecursoGlosaDto>>> GetPendingResponse()
        {
            var recursos = await _recursoService.GetPendingResponseAsync(GetTenantId());
            return Ok(recursos);
        }

        /// <summary>
        /// Get recursos by resultado
        /// </summary>
        [HttpGet("by-resultado/{resultado}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissRecursoGlosaDto>>> GetByResultado(string resultado)
        {
            if (!Enum.TryParse<ResultadoRecurso>(resultado, true, out var resultadoEnum))
            {
                return BadRequest(new { message = $"Resultado inválido: {resultado}" });
            }

            var recursos = await _recursoService.GetByResultadoAsync(resultadoEnum, GetTenantId());
            return Ok(recursos);
        }

        /// <summary>
        /// Create a new recurso (appeal)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissRecursoGlosaDto>> Create([FromBody] CreateTissRecursoGlosaDto dto)
        {
            try
            {
                var recurso = await _recursoService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = recurso.Id },
                    recurso);
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
        /// Register operator response to recurso
        /// </summary>
        [HttpPost("{id}/registrar-resposta")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissRecursoGlosaDto>> RegistrarResposta(
            Guid id,
            [FromBody] RegistrarRespostaDto dto)
        {
            if (!Enum.TryParse<ResultadoRecurso>(dto.Resultado, true, out var resultadoEnum))
            {
                return BadRequest(new { message = $"Resultado inválido: {dto.Resultado}" });
            }

            try
            {
                var recurso = await _recursoService.RegistrarRespostaAsync(
                    id,
                    resultadoEnum,
                    dto.JustificativaOperadora,
                    dto.ValorDeferido,
                    GetTenantId());

                return Ok(recurso);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a recurso
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _recursoService.DeleteAsync(id, GetTenantId());
            if (!success)
                return NotFound(new { message = "Recurso não encontrado." });

            return Ok(new { message = "Recurso excluído com sucesso." });
        }
    }

    /// <summary>
    /// DTO for registering operator response
    /// </summary>
    public class RegistrarRespostaDto
    {
        public string Resultado { get; set; } = string.Empty;
        public string? JustificativaOperadora { get; set; }
        public decimal? ValorDeferido { get; set; }
    }
}
