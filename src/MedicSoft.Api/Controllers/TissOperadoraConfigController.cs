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
    /// Controller for managing TISS operator webservice configurations
    /// </summary>
    [ApiController]
    [Route("api/tiss-operadora-configs")]
    [Authorize]
    public class TissOperadoraConfigController : BaseController
    {
        private readonly ITissOperadoraConfigService _configService;

        public TissOperadoraConfigController(
            ITissOperadoraConfigService configService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _configService = configService;
        }

        /// <summary>
        /// Get all operator configurations
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<IEnumerable<TissOperadoraConfigDto>>> GetAll()
        {
            var configs = await _configService.GetAllAsync(GetTenantId());
            return Ok(configs);
        }

        /// <summary>
        /// Get active operator configurations
        /// </summary>
        [HttpGet("active")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissOperadoraConfigDto>>> GetActive()
        {
            var configs = await _configService.GetActiveConfigsAsync(GetTenantId());
            return Ok(configs);
        }

        /// <summary>
        /// Get operator configuration by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissOperadoraConfigDto>> GetById(Guid id)
        {
            try
            {
                var config = await _configService.GetByIdAsync(id, GetTenantId());
                return Ok(config);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get operator configuration by operator ID
        /// </summary>
        [HttpGet("by-operator/{operatorId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissOperadoraConfigDto>> GetByOperatorId(Guid operatorId)
        {
            var config = await _configService.GetByOperatorIdAsync(operatorId, GetTenantId());
            if (config == null)
                return NotFound(new { message = "Configuração não encontrada para esta operadora." });

            return Ok(config);
        }

        /// <summary>
        /// Create a new operator configuration
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissOperadoraConfigDto>> Create([FromBody] CreateTissOperadoraConfigDto dto)
        {
            try
            {
                var config = await _configService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = config.Id },
                    config);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an operator configuration
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissOperadoraConfigDto>> Update(
            Guid id,
            [FromBody] CreateTissOperadoraConfigDto dto)
        {
            try
            {
                var config = await _configService.UpdateAsync(id, dto, GetTenantId());
                return Ok(config);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate an operator configuration
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult> Activate(Guid id)
        {
            var success = await _configService.ActivateAsync(id, GetTenantId());
            if (!success)
                return NotFound(new { message = "Configuração não encontrada." });

            return Ok(new { message = "Configuração ativada com sucesso." });
        }

        /// <summary>
        /// Deactivate an operator configuration
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            var success = await _configService.DeactivateAsync(id, GetTenantId());
            if (!success)
                return NotFound(new { message = "Configuração não encontrada." });

            return Ok(new { message = "Configuração desativada com sucesso." });
        }

        /// <summary>
        /// Delete an operator configuration
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _configService.DeleteAsync(id, GetTenantId());
            if (!success)
                return NotFound(new { message = "Configuração não encontrada." });

            return Ok(new { message = "Configuração excluída com sucesso." });
        }
    }
}
