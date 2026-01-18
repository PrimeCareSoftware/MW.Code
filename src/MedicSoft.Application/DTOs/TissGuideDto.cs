using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for TISS Guide
    /// </summary>
    public class TissGuideDto
    {
        public Guid Id { get; set; }
        public Guid TissBatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid AppointmentId { get; set; }
        public Guid PatientHealthInsuranceId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string GuideNumber { get; set; } = string.Empty;
        public string GuideType { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AuthorizationNumber { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public string? GlossReason { get; set; }
        public List<TissGuideProcedureDto> Procedures { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a TISS Guide
    /// </summary>
    public class CreateTissGuideDto
    {
        public Guid TissBatchId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientHealthInsuranceId { get; set; }
        public string GuideType { get; set; } = string.Empty; // Consultation, SPSADT, Hospitalization, Fees, Dental
        public DateTime ServiceDate { get; set; }
        public string? AuthorizationNumber { get; set; }
    }

    /// <summary>
    /// DTO for adding a procedure to a guide
    /// </summary>
    public class AddProcedureToGuideDto
    {
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureDescription { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// DTO for TISS Guide Procedure
    /// </summary>
    public class TissGuideProcedureDto
    {
        public Guid Id { get; set; }
        public Guid TissGuideId { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public string? GlossReason { get; set; }
    }

    /// <summary>
    /// DTO for processing operator response
    /// </summary>
    public class ProcessGuideResponseDto
    {
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public string? GlossReason { get; set; }
        public List<ProcedureResponseDto> ProcedureResponses { get; set; } = new();
    }

    /// <summary>
    /// DTO for individual procedure response
    /// </summary>
    public class ProcedureResponseDto
    {
        public Guid ProcedureId { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public string? GlossReason { get; set; }
    }
}
