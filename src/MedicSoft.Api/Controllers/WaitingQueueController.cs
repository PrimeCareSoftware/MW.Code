using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WaitingQueueController : BaseController
    {
        private readonly IWaitingQueueService _waitingQueueService;

        public WaitingQueueController(IWaitingQueueService waitingQueueService, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _waitingQueueService = waitingQueueService;
        }

        /// <summary>
        /// Get waiting queue summary for a clinic (requires authentication)
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<WaitingQueueSummaryDto>> GetQueueSummary([FromQuery] Guid clinicId)
        {
            if (clinicId == Guid.Empty)
            {
                return BadRequest(new { message = "clinicId is required and cannot be empty" });
            }

            try
            {
                var summary = await _waitingQueueService.GetQueueSummaryAsync(clinicId, GetTenantId());
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get public queue display for a clinic (no authentication required)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("public/{clinicId}")]
        public async Task<ActionResult<List<PublicQueueDisplayDto>>> GetPublicQueueDisplay(Guid clinicId, [FromQuery] string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId é obrigatório");

            try
            {
                var publicQueue = await _waitingQueueService.GetPublicQueueDisplayAsync(clinicId, tenantId);
                return Ok(publicQueue);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add a patient to the waiting queue
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WaitingQueueEntryDto>> AddToQueue([FromBody] CreateWaitingQueueEntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entry = await _waitingQueueService.AddToQueueAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetQueueSummary), new { clinicId = entry.ClinicId }, entry);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update triage priority for a queue entry
        /// </summary>
        [HttpPut("{entryId}/triage")]
        public async Task<ActionResult<WaitingQueueEntryDto>> UpdateTriage(Guid entryId, [FromBody] UpdateQueueTriageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entry = await _waitingQueueService.UpdateTriageAsync(entryId, dto, GetTenantId());
                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Call a patient from the queue
        /// </summary>
        [HttpPut("{entryId}/call")]
        public async Task<ActionResult<WaitingQueueEntryDto>> CallPatient(Guid entryId)
        {
            try
            {
                var entry = await _waitingQueueService.CallPatientAsync(entryId, GetTenantId());
                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Start service for a called patient
        /// </summary>
        [HttpPut("{entryId}/start")]
        public async Task<ActionResult<WaitingQueueEntryDto>> StartService(Guid entryId)
        {
            try
            {
                var entry = await _waitingQueueService.StartServiceAsync(entryId, GetTenantId());
                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Complete service for a patient
        /// </summary>
        [HttpPut("{entryId}/complete")]
        public async Task<ActionResult<WaitingQueueEntryDto>> CompleteService(Guid entryId)
        {
            try
            {
                var entry = await _waitingQueueService.CompleteServiceAsync(entryId, GetTenantId());
                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a queue entry
        /// </summary>
        [HttpDelete("{entryId}")]
        public async Task<ActionResult> CancelEntry(Guid entryId)
        {
            try
            {
                await _waitingQueueService.CancelEntryAsync(entryId, GetTenantId());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Get waiting queue configuration for a clinic
        /// </summary>
        [HttpGet("configuration")]
        public async Task<ActionResult<WaitingQueueConfigurationDto>> GetConfiguration([FromQuery] Guid clinicId)
        {
            try
            {
                var config = await _waitingQueueService.GetConfigurationAsync(clinicId, GetTenantId());
                
                if (config == null)
                    return NotFound("Configuração não encontrada para esta clínica");

                return Ok(config);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update waiting queue configuration for a clinic
        /// </summary>
        [HttpPut("configuration")]
        public async Task<ActionResult<WaitingQueueConfigurationDto>> UpdateConfiguration(
            [FromQuery] Guid clinicId, 
            [FromBody] UpdateWaitingQueueConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var config = await _waitingQueueService.UpdateConfigurationAsync(clinicId, dto, GetTenantId());
                return Ok(config);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
