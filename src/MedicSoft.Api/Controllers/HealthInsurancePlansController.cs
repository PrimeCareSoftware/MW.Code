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
    /// Controller for managing health insurance plans
    /// </summary>
    [ApiController]
    [Route("api/health-insurance-plans")]
    [Authorize]
    public class HealthInsurancePlansController : BaseController
    {
        private readonly IHealthInsurancePlanService _planService;

        public HealthInsurancePlansController(
            IHealthInsurancePlanService planService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _planService = planService;
        }

        /// <summary>
        /// Get all health insurance plans
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<IEnumerable<HealthInsurancePlanDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var plans = await _planService.GetAllAsync(GetTenantId(), includeInactive);
            return Ok(plans);
        }

        /// <summary>
        /// Get health insurance plan by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<HealthInsurancePlanDto>> GetById(Guid id)
        {
            var plan = await _planService.GetByIdAsync(id, GetTenantId());
            if (plan == null)
                return NotFound(new { message = $"Plano de saúde não encontrado." });

            return Ok(plan);
        }

        /// <summary>
        /// Get health insurance plans by operator ID
        /// </summary>
        [HttpGet("by-operator/{operatorId}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<IEnumerable<HealthInsurancePlanDto>>> GetByOperatorId(Guid operatorId)
        {
            var plans = await _planService.GetByOperatorIdAsync(operatorId, GetTenantId());
            return Ok(plans);
        }

        /// <summary>
        /// Get health insurance plan by plan code
        /// </summary>
        [HttpGet("by-code/{planCode}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceView)]
        public async Task<ActionResult<HealthInsurancePlanDto>> GetByPlanCode(string planCode)
        {
            var plan = await _planService.GetByPlanCodeAsync(planCode, GetTenantId());
            if (plan == null)
                return NotFound(new { message = $"Plano com código {planCode} não encontrado." });

            return Ok(plan);
        }

        /// <summary>
        /// Create a new health insurance plan
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceCreate)]
        public async Task<ActionResult<HealthInsurancePlanDto>> Create([FromBody] CreateHealthInsurancePlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var plan = await _planService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing health insurance plan
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsurancePlanDto>> Update(Guid id, [FromBody] UpdateHealthInsurancePlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var plan = await _planService.UpdateAsync(id, dto, GetTenantId());
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Enable TISS for a health insurance plan
        /// </summary>
        [HttpPost("{id}/enable-tiss")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsurancePlanDto>> EnableTiss(Guid id)
        {
            try
            {
                var plan = await _planService.EnableTissAsync(id, GetTenantId());
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Disable TISS for a health insurance plan
        /// </summary>
        [HttpPost("{id}/disable-tiss")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsurancePlanDto>> DisableTiss(Guid id)
        {
            try
            {
                var plan = await _planService.DisableTissAsync(id, GetTenantId());
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate a health insurance plan
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsurancePlanDto>> Activate(Guid id)
        {
            try
            {
                var plan = await _planService.ActivateAsync(id, GetTenantId());
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a health insurance plan
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceEdit)]
        public async Task<ActionResult<HealthInsurancePlanDto>> Deactivate(Guid id)
        {
            try
            {
                var plan = await _planService.DeactivateAsync(id, GetTenantId());
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a health insurance plan (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.HealthInsuranceDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _planService.DeleteAsync(id, GetTenantId());
                if (!result)
                    return NotFound(new { message = "Plano de saúde não encontrado." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
