using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Result of the complete payment flow orchestration
    /// Contains IDs of all created entities
    /// </summary>
    public class PaymentFlowResultDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? TissGuideId { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
