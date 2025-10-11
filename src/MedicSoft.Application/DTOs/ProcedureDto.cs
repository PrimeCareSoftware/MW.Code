using System;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class ProcedureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ProcedureCategory Category { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool RequiresMaterials { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProcedureDto
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ProcedureCategory Category { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool RequiresMaterials { get; set; }
    }

    public class UpdateProcedureDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ProcedureCategory Category { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool RequiresMaterials { get; set; }
    }

    public class AppointmentProcedureDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid ProcedureId { get; set; }
        public Guid PatientId { get; set; }
        public string ProcedureName { get; set; } = null!;
        public string ProcedureCode { get; set; } = null!;
        public decimal PriceCharged { get; set; }
        public string? Notes { get; set; }
        public DateTime PerformedAt { get; set; }
    }

    public class AddProcedureToAppointmentDto
    {
        public Guid ProcedureId { get; set; }
        public decimal? CustomPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentBillingSummaryDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public List<AppointmentProcedureDto> Procedures { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
