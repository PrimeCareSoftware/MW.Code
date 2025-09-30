using System;

namespace MedicSoft.Domain.Events
{
    public class AppointmentCancelledEvent : DomainEvent
    {
        public Guid AppointmentId { get; }
        public Guid PatientId { get; }
        public string? CancellationReason { get; }

        public AppointmentCancelledEvent(Guid appointmentId, Guid patientId, 
            string? cancellationReason, string tenantId) : base(tenantId)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            CancellationReason = cancellationReason;
        }
    }
}