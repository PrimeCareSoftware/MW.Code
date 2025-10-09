using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a procedure performed during an appointment.
    /// Links procedures to appointments and patients.
    /// </summary>
    public class AppointmentProcedure : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid ProcedureId { get; private set; }
        public Guid PatientId { get; private set; }
        public decimal PriceCharged { get; private set; }
        public string? Notes { get; private set; }
        public DateTime PerformedAt { get; private set; }

        // Navigation properties
        public Appointment? Appointment { get; private set; }
        public Procedure? Procedure { get; private set; }
        public Patient? Patient { get; private set; }

        private AppointmentProcedure()
        {
            // EF Constructor
        }

        public AppointmentProcedure(Guid appointmentId, Guid procedureId, Guid patientId,
            decimal priceCharged, DateTime performedAt, string tenantId, string? notes = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));

            if (procedureId == Guid.Empty)
                throw new ArgumentException("Procedure ID cannot be empty", nameof(procedureId));

            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            if (priceCharged < 0)
                throw new ArgumentException("Price charged cannot be negative", nameof(priceCharged));

            AppointmentId = appointmentId;
            ProcedureId = procedureId;
            PatientId = patientId;
            PriceCharged = priceCharged;
            PerformedAt = performedAt;
            Notes = notes?.Trim();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPrice));

            PriceCharged = newPrice;
            UpdateTimestamp();
        }
    }
}
