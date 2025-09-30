using System;

namespace MedicSoft.Domain.Events
{
    public class PatientCreatedEvent : DomainEvent
    {
        public Guid PatientId { get; }
        public string PatientName { get; }
        public string PatientEmail { get; }

        public PatientCreatedEvent(Guid patientId, string patientName, string patientEmail, string tenantId) 
            : base(tenantId)
        {
            PatientId = patientId;
            PatientName = patientName;
            PatientEmail = patientEmail;
        }
    }
}