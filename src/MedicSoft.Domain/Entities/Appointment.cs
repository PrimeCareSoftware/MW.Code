using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum AppointmentStatus
    {
        Scheduled = 1,
        Confirmed = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        NoShow = 6
    }

    public enum AppointmentType
    {
        Regular = 1,
        Emergency = 2,
        FollowUp = 3,
        Consultation = 4
    }

    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid ClinicId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public TimeSpan ScheduledTime { get; private set; }
        public int DurationMinutes { get; private set; }
        public AppointmentType Type { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime? CheckInTime { get; private set; }
        public DateTime? CheckOutTime { get; private set; }

        // Navigation properties
        public Patient Patient { get; private set; } = null!;
        public Clinic Clinic { get; private set; } = null!;

        private Appointment() { } // EF Constructor

        public Appointment(Guid patientId, Guid clinicId, DateTime scheduledDate, 
            TimeSpan scheduledTime, int durationMinutes, AppointmentType type, 
            string tenantId, string? notes = null) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (scheduledDate < DateTime.Today)
                throw new ArgumentException("Scheduled date cannot be in the past", nameof(scheduledDate));

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be positive", nameof(durationMinutes));

            PatientId = patientId;
            ClinicId = clinicId;
            ScheduledDate = scheduledDate;
            ScheduledTime = scheduledTime;
            DurationMinutes = durationMinutes;
            Type = type;
            Status = AppointmentStatus.Scheduled;
            Notes = notes?.Trim();
        }

        public void Confirm()
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled appointments can be confirmed");

            Status = AppointmentStatus.Confirmed;
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled appointments");

            Status = AppointmentStatus.Cancelled;
            CancellationReason = reason?.Trim();
            UpdateTimestamp();
        }

        public void MarkAsNoShow()
        {
            if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only scheduled or confirmed appointments can be marked as no-show");

            Status = AppointmentStatus.NoShow;
            UpdateTimestamp();
        }

        public void CheckIn()
        {
            if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Only confirmed or scheduled appointments can be checked in");

            Status = AppointmentStatus.InProgress;
            CheckInTime = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void CheckOut(string? notes = null)
        {
            if (Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException("Only in-progress appointments can be checked out");

            Status = AppointmentStatus.Completed;
            CheckOutTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes.Trim();
            UpdateTimestamp();
        }

        public void Reschedule(DateTime newDate, TimeSpan newTime)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot reschedule completed or cancelled appointments");

            if (newDate < DateTime.Today)
                throw new ArgumentException("New date cannot be in the past", nameof(newDate));

            ScheduledDate = newDate;
            ScheduledTime = newTime;
            Status = AppointmentStatus.Scheduled; // Reset to scheduled after reschedule
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public DateTime GetScheduledDateTime()
        {
            return ScheduledDate.Add(ScheduledTime);
        }

        public DateTime GetEndDateTime()
        {
            return GetScheduledDateTime().AddMinutes(DurationMinutes);
        }

        public bool IsOverlapping(DateTime start, DateTime end)
        {
            var appointmentStart = GetScheduledDateTime();
            var appointmentEnd = GetEndDateTime();
            
            return appointmentStart < end && appointmentEnd > start;
        }
    }
}