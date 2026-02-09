using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProcedurePricingConfigurationController : BaseController
    {
        private readonly IProcedurePricingConfigurationRepository _repository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IClinicRepository _clinicRepository;

        public ProcedurePricingConfigurationController(
            IProcedurePricingConfigurationRepository repository,
            IProcedureRepository procedureRepository,
            IClinicRepository clinicRepository,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _repository = repository;
            _procedureRepository = procedureRepository;
            _clinicRepository = clinicRepository;
        }

        /// <summary>
        /// Get procedure pricing configuration for a specific procedure at a clinic
        /// </summary>
        [HttpGet("procedure/{procedureId}/clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<ProcedurePricingConfiguration>> GetByProcedureAndClinic(Guid procedureId, Guid clinicId)
        {
            var tenantId = GetTenantId();
            var config = await _repository.GetByProcedureAndClinicAsync(procedureId, clinicId, tenantId);
            
            if (config == null)
                return NotFound($"Pricing configuration for procedure {procedureId} at clinic {clinicId} not found");

            return Ok(config);
        }

        /// <summary>
        /// Get all procedure pricing configurations for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<IEnumerable<ProcedurePricingConfiguration>>> GetByClinic(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var configs = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            return Ok(configs);
        }

        /// <summary>
        /// Create or update procedure pricing configuration
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.ProceduresEdit)]
        public async Task<ActionResult<ProcedurePricingConfiguration>> CreateOrUpdate([FromBody] CreateProcedurePricingConfigurationDto dto)
        {
            var tenantId = GetTenantId();
            
            // Verify procedure and clinic exist
            var procedure = await _procedureRepository.GetByIdAsync(dto.ProcedureId, tenantId);
            if (procedure == null)
                return NotFound($"Procedure {dto.ProcedureId} not found");

            var clinic = await _clinicRepository.GetByIdAsync(dto.ClinicId, tenantId);
            if (clinic == null)
                return NotFound($"Clinic {dto.ClinicId} not found");

            // Check if configuration already exists
            var existing = await _repository.GetByProcedureAndClinicAsync(dto.ProcedureId, dto.ClinicId, tenantId);
            
            if (existing != null)
            {
                // Update existing configuration
                if (dto.ConsultationPolicy.HasValue)
                {
                    existing.UpdateConsultationPolicy(
                        dto.ConsultationPolicy,
                        dto.ConsultationDiscountPercentage,
                        dto.ConsultationDiscountFixedAmount);
                }
                
                if (dto.CustomPrice.HasValue)
                {
                    existing.UpdateCustomPrice(dto.CustomPrice);
                }
                
                await _repository.UpdateAsync(existing);
                return Ok(existing);
            }
            else
            {
                // Create new configuration
                var config = new ProcedurePricingConfiguration(
                    dto.ProcedureId,
                    dto.ClinicId,
                    tenantId,
                    dto.ConsultationPolicy,
                    dto.CustomPrice);
                
                if (dto.ConsultationPolicy == ProcedureConsultationPolicy.DiscountOnConsultation)
                {
                    config.UpdateConsultationPolicy(
                        dto.ConsultationPolicy,
                        dto.ConsultationDiscountPercentage,
                        dto.ConsultationDiscountFixedAmount);
                }
                
                await _repository.AddAsync(config);
                return CreatedAtAction(
                    nameof(GetByProcedureAndClinic), 
                    new { procedureId = dto.ProcedureId, clinicId = dto.ClinicId }, 
                    config);
            }
        }

        /// <summary>
        /// Delete procedure pricing configuration
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.ProceduresEdit)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var config = await _repository.GetByIdAsync(id, tenantId);
            
            if (config == null)
                return NotFound($"Configuration {id} not found");

            await _repository.DeleteAsync(config.Id, tenantId);
            return NoContent();
        }
    }

    public class CreateProcedurePricingConfigurationDto
    {
        public Guid ProcedureId { get; set; }
        public Guid ClinicId { get; set; }
        public ProcedureConsultationPolicy? ConsultationPolicy { get; set; }
        public decimal? ConsultationDiscountPercentage { get; set; }
        public decimal? ConsultationDiscountFixedAmount { get; set; }
        public decimal? CustomPrice { get; set; }
    }
}
