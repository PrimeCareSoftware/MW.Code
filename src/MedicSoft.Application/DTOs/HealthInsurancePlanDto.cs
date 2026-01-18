using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Health Insurance Plan
    /// </summary>
    public class HealthInsurancePlanDto
    {
        public Guid Id { get; set; }
        public Guid OperatorId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanCode { get; set; } = string.Empty;
        public string? RegisterNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool CoversConsultations { get; set; }
        public bool CoversExams { get; set; }
        public bool CoversProcedures { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
        public bool TissEnabled { get; set; }
        public string? OperatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a Health Insurance Plan
    /// </summary>
    public class CreateHealthInsurancePlanDto
    {
        public Guid OperatorId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanCode { get; set; } = string.Empty;
        public string? RegisterNumber { get; set; }
        public string Type { get; set; } = "Individual"; // Individual, Enterprise, Collective
        public bool CoversConsultations { get; set; } = true;
        public bool CoversExams { get; set; } = true;
        public bool CoversProcedures { get; set; } = true;
        public bool RequiresPriorAuthorization { get; set; } = false;
        public bool TissEnabled { get; set; } = false;
    }

    /// <summary>
    /// DTO for updating a Health Insurance Plan
    /// </summary>
    public class UpdateHealthInsurancePlanDto
    {
        public string PlanName { get; set; } = string.Empty;
        public string PlanCode { get; set; } = string.Empty;
        public string? RegisterNumber { get; set; }
        public string Type { get; set; } = "Individual";
        public bool CoversConsultations { get; set; }
        public bool CoversExams { get; set; }
        public bool CoversProcedures { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
    }
}
