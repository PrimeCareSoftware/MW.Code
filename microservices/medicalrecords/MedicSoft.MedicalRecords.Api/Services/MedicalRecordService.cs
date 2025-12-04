using Microsoft.EntityFrameworkCore;
using MedicSoft.MedicalRecords.Api.Data;
using MedicSoft.MedicalRecords.Api.Models;

namespace MedicSoft.MedicalRecords.Api.Services;

public interface IMedicalRecordService
{
    Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto, string tenantId);
    Task<MedicalRecordDto?> UpdateMedicalRecordAsync(Guid id, UpdateMedicalRecordDto dto, string tenantId);
    Task<MedicalRecordDto?> CompleteMedicalRecordAsync(Guid id, CompleteMedicalRecordDto dto, string tenantId);
    Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
    Task<IEnumerable<MedicalRecordDto>> GetPatientMedicalRecordsAsync(Guid patientId, string tenantId);
}

public interface IMedicationService
{
    Task<IEnumerable<MedicationDto>> GetAllAsync(string tenantId, bool activeOnly = true);
    Task<IEnumerable<MedicationDto>> SearchByNameAsync(string term, string tenantId);
    Task<MedicationDto?> GetByIdAsync(Guid id, string tenantId);
    Task<MedicationDto> CreateAsync(CreateMedicationDto dto, string tenantId);
}

public class MedicalRecordService : IMedicalRecordService
{
    private readonly MedicalRecordsDbContext _context;
    private readonly ILogger<MedicalRecordService> _logger;

    public MedicalRecordService(MedicalRecordsDbContext context, ILogger<MedicalRecordService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto, string tenantId)
    {
        // Check if medical record already exists for this appointment
        var existing = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.AppointmentId == dto.AppointmentId && m.TenantId == tenantId);

        if (existing != null)
        {
            throw new InvalidOperationException("A medical record already exists for this appointment");
        }

        var record = new MedicalRecordEntity
        {
            Id = Guid.NewGuid(),
            AppointmentId = dto.AppointmentId,
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ChiefComplaint = dto.ChiefComplaint,
            HistoryOfPresentIllness = dto.HistoryOfPresentIllness,
            PhysicalExamination = dto.PhysicalExamination,
            Diagnosis = dto.Diagnosis,
            TreatmentPlan = dto.TreatmentPlan,
            Notes = dto.Notes,
            VitalSigns = dto.VitalSigns,
            IsCompleted = false,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.MedicalRecords.Add(record);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created medical record: {RecordId} for appointment: {AppointmentId}", record.Id, dto.AppointmentId);
        return await GetRecordWithDetailsAsync(record);
    }

    public async Task<MedicalRecordDto?> UpdateMedicalRecordAsync(Guid id, UpdateMedicalRecordDto dto, string tenantId)
    {
        var record = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId);

        if (record == null)
            return null;

        if (record.IsCompleted)
            throw new InvalidOperationException("Cannot update a completed medical record");

        if (dto.ChiefComplaint != null) record.ChiefComplaint = dto.ChiefComplaint;
        if (dto.HistoryOfPresentIllness != null) record.HistoryOfPresentIllness = dto.HistoryOfPresentIllness;
        if (dto.PhysicalExamination != null) record.PhysicalExamination = dto.PhysicalExamination;
        if (dto.Diagnosis != null) record.Diagnosis = dto.Diagnosis;
        if (dto.TreatmentPlan != null) record.TreatmentPlan = dto.TreatmentPlan;
        if (dto.Notes != null) record.Notes = dto.Notes;
        if (dto.VitalSigns != null) record.VitalSigns = dto.VitalSigns;

        record.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated medical record: {RecordId}", id);
        return await GetRecordWithDetailsAsync(record);
    }

    public async Task<MedicalRecordDto?> CompleteMedicalRecordAsync(Guid id, CompleteMedicalRecordDto dto, string tenantId)
    {
        var record = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId);

        if (record == null)
            return null;

        if (dto.FinalDiagnosis != null) record.Diagnosis = dto.FinalDiagnosis;
        if (dto.TreatmentPlan != null) record.TreatmentPlan = dto.TreatmentPlan;
        if (dto.Notes != null) record.Notes = dto.Notes;

        record.IsCompleted = true;
        record.CompletedAt = DateTime.UtcNow;
        record.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Completed medical record: {RecordId}", id);
        return await GetRecordWithDetailsAsync(record);
    }

    public async Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
    {
        var record = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId && m.TenantId == tenantId);

        return record != null ? await GetRecordWithDetailsAsync(record) : null;
    }

    public async Task<IEnumerable<MedicalRecordDto>> GetPatientMedicalRecordsAsync(Guid patientId, string tenantId)
    {
        var records = await _context.MedicalRecords
            .Where(m => m.PatientId == patientId && m.TenantId == tenantId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var results = new List<MedicalRecordDto>();
        foreach (var record in records)
        {
            results.Add(await GetRecordWithDetailsAsync(record));
        }
        return results;
    }

    private async Task<MedicalRecordDto> GetRecordWithDetailsAsync(MedicalRecordEntity record)
    {
        var prescriptionItems = await _context.PrescriptionItems
            .Where(p => p.MedicalRecordId == record.Id)
            .ToListAsync();

        var examRequests = await _context.ExamRequests
            .Where(e => e.MedicalRecordId == record.Id)
            .ToListAsync();

        return new MedicalRecordDto
        {
            Id = record.Id,
            AppointmentId = record.AppointmentId,
            PatientId = record.PatientId,
            DoctorId = record.DoctorId,
            ChiefComplaint = record.ChiefComplaint,
            HistoryOfPresentIllness = record.HistoryOfPresentIllness,
            PhysicalExamination = record.PhysicalExamination,
            Diagnosis = record.Diagnosis,
            TreatmentPlan = record.TreatmentPlan,
            Notes = record.Notes,
            VitalSigns = record.VitalSigns,
            IsCompleted = record.IsCompleted,
            CompletedAt = record.CompletedAt,
            CreatedAt = record.CreatedAt,
            PrescriptionItems = prescriptionItems.Select(p => new PrescriptionItemDto
            {
                Id = p.Id,
                MedicationId = p.MedicationId,
                MedicationName = p.MedicationName,
                Dosage = p.Dosage,
                Frequency = p.Frequency,
                DurationDays = p.DurationDays,
                Instructions = p.Instructions
            }),
            ExamRequests = examRequests.Select(e => new ExamRequestDto
            {
                Id = e.Id,
                PatientId = e.PatientId,
                ExamName = e.ExamName,
                ExamCode = e.ExamCode,
                Instructions = e.Instructions,
                ClinicalIndication = e.ClinicalIndication,
                Status = GetExamStatusName(e.Status),
                ResultDate = e.ResultDate,
                ResultNotes = e.ResultNotes,
                CreatedAt = e.CreatedAt
            })
        };
    }

    private static string GetExamStatusName(int status)
    {
        return status switch
        {
            0 => "Requested",
            1 => "Scheduled",
            2 => "Completed",
            3 => "Cancelled",
            _ => "Unknown"
        };
    }
}

public class MedicationService : IMedicationService
{
    private readonly MedicalRecordsDbContext _context;
    private readonly ILogger<MedicationService> _logger;

    public MedicationService(MedicalRecordsDbContext context, ILogger<MedicationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicationDto>> GetAllAsync(string tenantId, bool activeOnly = true)
    {
        var query = _context.Medications.Where(m => m.TenantId == tenantId);
        if (activeOnly)
            query = query.Where(m => m.IsActive);

        var medications = await query.OrderBy(m => m.Name).ToListAsync();
        return medications.Select(MapToDto);
    }

    public async Task<IEnumerable<MedicationDto>> SearchByNameAsync(string term, string tenantId)
    {
        var medications = await _context.Medications
            .Where(m => m.TenantId == tenantId && m.IsActive &&
                (m.Name.ToLower().Contains(term.ToLower()) ||
                 (m.GenericName != null && m.GenericName.ToLower().Contains(term.ToLower()))))
            .OrderBy(m => m.Name)
            .Take(20)
            .ToListAsync();

        return medications.Select(MapToDto);
    }

    public async Task<MedicationDto?> GetByIdAsync(Guid id, string tenantId)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId);

        return medication != null ? MapToDto(medication) : null;
    }

    public async Task<MedicationDto> CreateAsync(CreateMedicationDto dto, string tenantId)
    {
        var medication = new MedicationEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            GenericName = dto.GenericName,
            Manufacturer = dto.Manufacturer,
            ActiveIngredient = dto.ActiveIngredient,
            Dosage = dto.Dosage,
            PharmaceuticalForm = dto.PharmaceuticalForm,
            Concentration = dto.Concentration,
            AdministrationRoute = dto.AdministrationRoute,
            Category = dto.Category,
            RequiresPrescription = dto.RequiresPrescription,
            IsControlled = dto.IsControlled,
            AnvisaRegistration = dto.AnvisaRegistration,
            Barcode = dto.Barcode,
            Description = dto.Description,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created medication: {MedicationId} - {Name}", medication.Id, medication.Name);
        return MapToDto(medication);
    }

    private static MedicationDto MapToDto(MedicationEntity entity)
    {
        return new MedicationDto
        {
            Id = entity.Id,
            Name = entity.Name,
            GenericName = entity.GenericName,
            Manufacturer = entity.Manufacturer,
            Dosage = entity.Dosage,
            PharmaceuticalForm = entity.PharmaceuticalForm,
            AdministrationRoute = entity.AdministrationRoute,
            Category = GetCategoryName(entity.Category),
            RequiresPrescription = entity.RequiresPrescription,
            IsControlled = entity.IsControlled,
            IsActive = entity.IsActive
        };
    }

    private static string GetCategoryName(int category)
    {
        return category switch
        {
            0 => "Generic",
            1 => "Brand",
            2 => "Controlled",
            3 => "OTC",
            _ => "Unknown"
        };
    }
}
