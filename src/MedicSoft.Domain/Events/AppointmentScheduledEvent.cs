using System;

namespace MedicSoft.Domain.Events
{
    public class AppointmentScheduledEvent : DomainEvent
    {
        public Guid AppointmentId { get; }
        public Guid PatientId { get; }
        public Guid ClinicId { get; }
        public DateTime ScheduledDateTime { get; }

        public AppointmentScheduledEvent(Guid appointmentId, Guid patientId, Guid clinicId, 
            DateTime scheduledDateTime, string tenantId) : base(tenantId)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            ClinicId = clinicId;
            ScheduledDateTime = scheduledDateTime;
        }
    }
}