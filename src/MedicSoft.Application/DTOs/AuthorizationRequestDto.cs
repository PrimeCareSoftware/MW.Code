using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Authorization Request
    /// </summary>
    public class AuthorizationRequestDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid PatientHealthInsuranceId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string OperatorName { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AuthorizationNumber { get; set; }
        public DateTime? AuthorizationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? DenialReason { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? ClinicalIndication { get; set; }
        public string? Diagnosis { get; set; }
        public bool IsExpired { get; set; }
        public bool IsValidForUse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating an authorization request
    /// </summary>
    public class CreateAuthorizationRequestDto
    {
        public Guid PatientId { get; set; }
        public Guid PatientHealthInsuranceId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureDescription { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string? ClinicalIndication { get; set; }
        public string? Diagnosis { get; set; }
    }

    /// <summary>
    /// DTO for approving an authorization request
    /// </summary>
    public class ApproveAuthorizationDto
    {
        public string AuthorizationNumber { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
    }

    /// <summary>
    /// DTO for denying an authorization request
    /// </summary>
    public class DenyAuthorizationDto
    {
        public string DenialReason { get; set; } = string.Empty;
    }
}
