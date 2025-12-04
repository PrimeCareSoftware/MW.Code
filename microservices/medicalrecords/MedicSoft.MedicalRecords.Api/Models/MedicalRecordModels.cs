namespace MedicSoft.MedicalRecords.Api.Models;

public class MedicalRecordDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string? HistoryOfPresentIllness { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
    public string? VitalSigns { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<PrescriptionItemDto> PrescriptionItems { get; set; } = new List<PrescriptionItemDto>();
    public IEnumerable<ExamRequestDto> ExamRequests { get; set; } = new List<ExamRequestDto>();
}

public class CreateMedicalRecordDto
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string? HistoryOfPresentIllness { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
    public string? VitalSigns { get; set; }
}

public class UpdateMedicalRecordDto
{
    public string? ChiefComplaint { get; set; }
    public string? HistoryOfPresentIllness { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
    public string? VitalSigns { get; set; }
}

public class CompleteMedicalRecordDto
{
    public string? FinalDiagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
}

public class MedicationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? Manufacturer { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string PharmaceuticalForm { get; set; } = string.Empty;
    public string? AdministrationRoute { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMedicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? Manufacturer { get; set; }
    public string? ActiveIngredient { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string PharmaceuticalForm { get; set; } = string.Empty;
    public string? Concentration { get; set; }
    public string? AdministrationRoute { get; set; }
    public int Category { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public string? AnvisaRegistration { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
}

public class PrescriptionItemDto
{
    public Guid Id { get; set; }
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }
}

public class ExamRequestDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string? ExamCode { get; set; }
    public string? Instructions { get; set; }
    public string? ClinicalIndication { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ResultDate { get; set; }
    public string? ResultNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExamRequestDto
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? MedicalRecordId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string? ExamCode { get; set; }
    public string? Instructions { get; set; }
    public string? ClinicalIndication { get; set; }
}
