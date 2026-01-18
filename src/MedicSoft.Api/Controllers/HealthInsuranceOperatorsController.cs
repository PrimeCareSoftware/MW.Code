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
    /// Controller for managing health insurance operators
    /// </summary>
    [ApiController]
    [Route("api/health-insurance-operators")]
    [Authorize]
    public class HealthInsuranceOperatorsController : BaseController
    {
        private readonly IHealthInsuranceOperatorService _operatorService;

        public HealthInsuranceOperatorsController(
            IHealthInsuranceOperatorService operatorService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _operatorService = operatorService;
        }

        /// <summary>
        /// Get all health insurance operators
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<IEnumerable<HealthInsuranceOperatorDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var operators = await _operatorService.GetAllAsync(GetTenantId(), includeInactive);
            return Ok(operators);
        }

        /// <summary>
        /// Get health insurance operator by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> GetById(Guid id)
        {
            var operatorDto = await _operatorService.GetByIdAsync(id, GetTenantId());
            if (operatorDto == null)
                return NotFound(new { message = $"Operadora não encontrada." });

            return Ok(operatorDto);
        }

        /// <summary>
        /// Get health insurance operator by ANS register number
        /// </summary>
        [HttpGet("by-register/{registerNumber}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> GetByRegisterNumber(string registerNumber)
        {
            var operatorDto = await _operatorService.GetByRegisterNumberAsync(registerNumber, GetTenantId());
            if (operatorDto == null)
                return NotFound(new { message = $"Operadora com registro ANS {registerNumber} não encontrada." });

            return Ok(operatorDto);
        }

        /// <summary>
        /// Search operators by name
        /// </summary>
        [HttpGet("search")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<IEnumerable<HealthInsuranceOperatorDto>>> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                return BadRequest(new { message = "O termo de busca deve ter pelo menos 3 caracteres." });

            var operators = await _operatorService.SearchByNameAsync(name, GetTenantId());
            return Ok(operators);
        }

        /// <summary>
        /// Create a new health insurance operator
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceCreate)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> Create([FromBody] CreateHealthInsuranceOperatorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var operatorDto = await _operatorService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = operatorDto.Id }, operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing health insurance operator
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> Update(Guid id, [FromBody] UpdateHealthInsuranceOperatorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var operatorDto = await _operatorService.UpdateAsync(id, dto, GetTenantId());
                return Ok(operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Configure operator integration settings
        /// </summary>
        [HttpPost("{id}/configure-integration")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> ConfigureIntegration(Guid id, [FromBody] ConfigureOperatorIntegrationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var operatorDto = await _operatorService.ConfigureIntegrationAsync(id, dto, GetTenantId());
                return Ok(operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Configure TISS settings for the operator
        /// </summary>
        [HttpPost("{id}/configure-tiss")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> ConfigureTiss(Guid id, [FromBody] ConfigureOperatorTissDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var operatorDto = await _operatorService.ConfigureTissAsync(id, dto, GetTenantId());
                return Ok(operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate an operator
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> Activate(Guid id)
        {
            try
            {
                var operatorDto = await _operatorService.ActivateAsync(id, GetTenantId());
                return Ok(operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate an operator
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsuranceOperatorDto>> Deactivate(Guid id)
        {
            try
            {
                var operatorDto = await _operatorService.DeactivateAsync(id, GetTenantId());
                return Ok(operatorDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an operator (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _operatorService.DeleteAsync(id, GetTenantId());
                if (!result)
                    return NotFound(new { message = "Operadora não encontrada." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
