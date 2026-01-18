using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class FinancialClosureDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? HealthInsuranceOperatorId { get; set; }
        public string ClosureNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public DateTime ClosureDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PatientAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string? CancellationReason { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? DiscountReason { get; set; }
        public List<FinancialClosureItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FinancialClosureItemDto
    {
        public Guid Id { get; set; }
        public Guid ClosureId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool CoverByInsurance { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFinancialClosureDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? HealthInsuranceOperatorId { get; set; }
        public string ClosureNumber { get; set; } = string.Empty;
        public int PaymentType { get; set; } // ClosurePaymentType enum value
    }

    public class AddClosureItemDto
    {
        public Guid ClosureId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CoverByInsurance { get; set; }
    }

    public class ApplyClosureDiscountDto
    {
        public Guid ClosureId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class RecordClosurePaymentDto
    {
        public Guid ClosureId { get; set; }
        public decimal Amount { get; set; }
    }

    public class CancelClosureDto
    {
        public Guid ClosureId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
