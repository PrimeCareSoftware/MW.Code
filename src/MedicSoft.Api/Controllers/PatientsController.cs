using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    public class PatientsController : BaseController
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Get all patients
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patients = await _patientService.GetAllPatientsAsync(GetTenantId());
            return Ok(patients);
        }

        /// <summary>
        /// Search patients by CPF, Name or Phone
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty");

            var patients = await _patientService.SearchPatientsAsync(searchTerm, GetTenantId());
            return Ok(patients);
        }

        /// <summary>
        /// Get patient by document (CPF) across all clinics
        /// </summary>
        [HttpGet("by-document/{document}")]
        public async Task<ActionResult<PatientDto>> GetByDocument(string document)
        {
            var patient = await _patientService.GetPatientByDocumentGlobalAsync(document);
            
            if (patient == null)
                return NotFound($"Patient with document {document} not found");

            return Ok(patient);
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id, GetTenantId());
            
            if (patient == null)
                return NotFound($"Patient with ID {id} not found");

            return Ok(patient);
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto createPatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var patient = await _patientService.CreatePatientAsync(createPatientDto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing patient
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var patient = await _patientService.UpdatePatientAsync(id, updatePatientDto, GetTenantId());
                return Ok(patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a patient
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(id, GetTenantId());
                
                if (!result)
                    return NotFound($"Patient with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Link patient to a clinic
        /// </summary>
        [HttpPost("{patientId}/link-clinic/{clinicId}")]
        public async Task<ActionResult> LinkToClinic(Guid patientId, Guid clinicId)
        {
            try
            {
                var result = await _patientService.LinkPatientToClinicAsync(patientId, clinicId, GetTenantId());
                return Ok(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Link child to guardian
        /// </summary>
        [HttpPost("{childId}/link-guardian/{guardianId}")]
        public async Task<ActionResult> LinkChildToGuardian(Guid childId, Guid guardianId)
        {
            try
            {
                var result = await _patientService.LinkChildToGuardianAsync(childId, guardianId, GetTenantId());
                return Ok(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get children of a guardian
        /// </summary>
        [HttpGet("{guardianId}/children")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetChildren(Guid guardianId)
        {
            var children = await _patientService.GetChildrenOfGuardianAsync(guardianId, GetTenantId());
            return Ok(children);
        }
    }
}