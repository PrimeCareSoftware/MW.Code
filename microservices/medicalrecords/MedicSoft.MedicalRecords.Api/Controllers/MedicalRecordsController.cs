using Microsoft.AspNetCore.Mvc;
using MedicSoft.MedicalRecords.Api.Models;
using MedicSoft.MedicalRecords.Api.Services;
using MedicSoft.Shared.Authentication;

namespace MedicSoft.MedicalRecords.Api.Controllers;

[Route("api/[controller]")]
public class MedicalRecordsController : MicroserviceBaseController
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    /// <summary>
    /// Create a new medical record for an appointment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MedicalRecordDto>> Create([FromBody] CreateMedicalRecordDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var medicalRecord = await _medicalRecordService.CreateMedicalRecordAsync(createDto, GetTenantId());
            return CreatedAtAction(nameof(GetByAppointment),
                new { appointmentId = medicalRecord.AppointmentId },
                medicalRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update a medical record
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MedicalRecordDto>> Update(Guid id, [FromBody] UpdateMedicalRecordDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var medicalRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, updateDto, GetTenantId());
            if (medicalRecord == null)
                return NotFound($"Medical record with ID {id} not found");
            return Ok(medicalRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Complete a medical record and finish consultation
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<MedicalRecordDto>> Complete(Guid id, [FromBody] CompleteMedicalRecordDto completeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var medicalRecord = await _medicalRecordService.CompleteMedicalRecordAsync(id, completeDto, GetTenantId());
            if (medicalRecord == null)
                return NotFound($"Medical record with ID {id} not found");
            return Ok(medicalRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get medical record by appointment ID
    /// </summary>
    [HttpGet("appointment/{appointmentId}")]
    public async Task<ActionResult<MedicalRecordDto>> GetByAppointment(Guid appointmentId)
    {
        try
        {
            var medicalRecord = await _medicalRecordService.GetByAppointmentIdAsync(appointmentId, GetTenantId());

            if (medicalRecord == null)
                return NotFound($"Medical record not found for appointment {appointmentId}");

            return Ok(medicalRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get all medical records for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetByPatient(Guid patientId)
    {
        try
        {
            var medicalRecords = await _medicalRecordService.GetPatientMedicalRecordsAsync(patientId, GetTenantId());
            return Ok(medicalRecords);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "MedicalRecords.Microservice" });
    }
}

[Route("api/[controller]")]
public class MedicationsController : MicroserviceBaseController
{
    private readonly IMedicationService _medicationService;

    public MedicationsController(IMedicationService medicationService)
    {
        _medicationService = medicationService;
    }

    /// <summary>
    /// Get all active medications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll([FromQuery] bool activeOnly = true)
    {
        var medications = await _medicationService.GetAllAsync(GetTenantId(), activeOnly);
        return Ok(medications);
    }

    /// <summary>
    /// Search medications by name
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MedicationDto>>> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Ok(Array.Empty<MedicationDto>());

        var medications = await _medicationService.SearchByNameAsync(term, GetTenantId());
        return Ok(medications);
    }

    /// <summary>
    /// Get medication by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MedicationDto>> GetById(Guid id)
    {
        var medication = await _medicationService.GetByIdAsync(id, GetTenantId());

        if (medication == null)
            return NotFound($"Medication with ID {id} not found");

        return Ok(medication);
    }

    /// <summary>
    /// Create a new medication
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MedicationDto>> Create([FromBody] CreateMedicationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var medication = await _medicationService.CreateAsync(dto, GetTenantId());
            return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Medications.Microservice" });
    }
}
