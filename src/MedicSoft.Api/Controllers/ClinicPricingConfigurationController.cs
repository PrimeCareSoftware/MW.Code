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
    public class ClinicPricingConfigurationController : BaseController
    {
        private readonly IClinicPricingConfigurationRepository _repository;
        private readonly IClinicRepository _clinicRepository;

        public ClinicPricingConfigurationController(
            IClinicPricingConfigurationRepository repository,
            IClinicRepository clinicRepository,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _repository = repository;
            _clinicRepository = clinicRepository;
        }

        /// <summary>
        /// Get pricing configuration for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult<ClinicPricingConfiguration>> GetByClinic(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var config = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            
            if (config == null)
                return NotFound($"Pricing configuration for clinic {clinicId} not found");

            return Ok(config);
        }

        /// <summary>
        /// Create or update pricing configuration for a clinic
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult<ClinicPricingConfiguration>> CreateOrUpdate([FromBody] CreateClinicPricingConfigurationDto dto)
        {
            var tenantId = GetTenantId();
            
            // Verify clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(dto.ClinicId, tenantId);
            if (clinic == null)
                return NotFound($"Clinic {dto.ClinicId} not found");

            // Check if configuration already exists
            var existing = await _repository.GetByClinicIdAsync(dto.ClinicId, tenantId);
            
            if (existing != null)
            {
                // Update existing configuration
                existing.UpdateConsultationPrices(
                    dto.DefaultConsultationPrice,
                    dto.FollowUpConsultationPrice,
                    dto.TelemedicineConsultationPrice);
                
                existing.UpdateProcedurePolicy(
                    dto.DefaultProcedurePolicy,
                    dto.ConsultationDiscountPercentage,
                    dto.ConsultationDiscountFixedAmount);
                
                await _repository.UpdateAsync(existing);
                return Ok(existing);
            }
            else
            {
                // Create new configuration
                var config = new ClinicPricingConfiguration(
                    dto.ClinicId,
                    dto.DefaultConsultationPrice,
                    tenantId,
                    dto.DefaultProcedurePolicy,
                    dto.FollowUpConsultationPrice,
                    dto.TelemedicineConsultationPrice);
                
                if (dto.DefaultProcedurePolicy == ProcedureConsultationPolicy.DiscountOnConsultation)
                {
                    config.UpdateProcedurePolicy(
                        dto.DefaultProcedurePolicy,
                        dto.ConsultationDiscountPercentage,
                        dto.ConsultationDiscountFixedAmount);
                }
                
                await _repository.AddAsync(config);
                return CreatedAtAction(nameof(GetByClinic), new { clinicId = dto.ClinicId }, config);
            }
        }
    }

    public class CreateClinicPricingConfigurationDto
    {
        public Guid ClinicId { get; set; }
        public decimal DefaultConsultationPrice { get; set; }
        public decimal? FollowUpConsultationPrice { get; set; }
        public decimal? TelemedicineConsultationPrice { get; set; }
        public ProcedureConsultationPolicy DefaultProcedurePolicy { get; set; }
        public decimal? ConsultationDiscountPercentage { get; set; }
        public decimal? ConsultationDiscountFixedAmount { get; set; }
    }
}
