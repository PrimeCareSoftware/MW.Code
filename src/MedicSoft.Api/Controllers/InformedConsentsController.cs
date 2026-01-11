using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing Informed Consents (CFM 1.821)
    /// </summary>
    [ApiController]
    [Route("api/informed-consents")]
    public class InformedConsentsController : BaseController
    {
        private readonly IInformedConsentService _informedConsentService;

        public InformedConsentsController(
            IInformedConsentService informedConsentService, 
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _informedConsentService = informedConsentService;
        }

        /// <summary>
        /// Create a new informed consent for a medical record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InformedConsentDto>> Create([FromBody] CreateInformedConsentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var consent = await _informedConsentService.CreateInformedConsentAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetByMedicalRecord), 
                    new { medicalRecordId = consent.MedicalRecordId }, 
                    consent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Accept an informed consent (patient acceptance with IP and optional signature)
        /// </summary>
        [HttpPost("{id}/accept")]
        public async Task<ActionResult<InformedConsentDto>> Accept(Guid id, [FromBody] AcceptInformedConsentDto acceptDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Get IP address from request if not provided
                var ipAddress = string.IsNullOrEmpty(acceptDto.IPAddress) 
                    ? HttpContext.Connection.RemoteIpAddress?.ToString() 
                    : acceptDto.IPAddress;

                var updatedAcceptDto = new AcceptInformedConsentDto 
                { 
                    IPAddress = ipAddress,
                    DigitalSignature = acceptDto.DigitalSignature
                };

                var consent = await _informedConsentService.AcceptInformedConsentAsync(id, updatedAcceptDto, GetTenantId());
                return Ok(consent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all informed consents for a medical record
        /// </summary>
        [HttpGet("medical-record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<InformedConsentDto>>> GetByMedicalRecord(Guid medicalRecordId)
        {
            try
            {
                var consents = await _informedConsentService.GetByMedicalRecordIdAsync(medicalRecordId, GetTenantId());
                return Ok(consents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
