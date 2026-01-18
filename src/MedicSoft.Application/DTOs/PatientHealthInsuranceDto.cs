using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Patient Health Insurance
    /// </summary>
    public class PatientHealthInsuranceDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid HealthInsurancePlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string OperatorName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string? CardValidationCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public bool IsHolder { get; set; }
        public string? HolderName { get; set; }
        public string? HolderDocument { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for linking a patient to a health plan
    /// </summary>
    public class CreatePatientHealthInsuranceDto
    {
        public Guid PatientId { get; set; }
        public Guid HealthInsurancePlanId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string? CardValidationCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsHolder { get; set; } = true;
        public string? HolderName { get; set; }
        public string? HolderDocument { get; set; }
    }

    /// <summary>
    /// DTO for updating patient health insurance
    /// </summary>
    public class UpdatePatientHealthInsuranceDto
    {
        public string CardNumber { get; set; } = string.Empty;
        public string? CardValidationCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsHolder { get; set; }
        public string? HolderName { get; set; }
        public string? HolderDocument { get; set; }
    }

    /// <summary>
    /// DTO for card validation result
    /// </summary>
    public class CardValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public PatientHealthInsuranceDto? Insurance { get; set; }
    }
}
