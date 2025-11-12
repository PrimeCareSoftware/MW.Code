using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a request for medical exams or diagnostic tests
    /// Created during appointments by doctors/dentists
    /// </summary>
    public class ExamRequest : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        public ExamType ExamType { get; private set; }
        public string ExamName { get; private set; }
        public string Description { get; private set; }
        public ExamUrgency Urgency { get; private set; }
        public ExamRequestStatus Status { get; private set; }
        public DateTime RequestedDate { get; private set; }
        public DateTime? ScheduledDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public string? Results { get; private set; }
        public string? Notes { get; private set; }

        // Navigation properties
        public Appointment? Appointment { get; private set; }
        public Patient? Patient { get; private set; }

        private ExamRequest()
        {
            // EF Constructor
            ExamName = null!;
            Description = null!;
        }

        public ExamRequest(Guid appointmentId, Guid patientId, ExamType examType,
            string examName, string description, ExamUrgency urgency,
            string tenantId, string? notes = null, DateTime? requestedDate = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));

            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            if (string.IsNullOrWhiteSpace(examName))
                throw new ArgumentException("Exam name cannot be empty", nameof(examName));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            AppointmentId = appointmentId;
            PatientId = patientId;
            ExamType = examType;
            ExamName = examName.Trim();
            Description = description.Trim();
            Urgency = urgency;
            Status = ExamRequestStatus.Pending;
            RequestedDate = requestedDate ?? DateTime.UtcNow;
            Notes = notes?.Trim();
        }

        public void Update(string examName, string description, ExamUrgency urgency, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(examName))
                throw new ArgumentException("Exam name cannot be empty", nameof(examName));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            ExamName = examName.Trim();
            Description = description.Trim();
            Urgency = urgency;
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public void Schedule(DateTime scheduledDate, bool allowHistoricalData = false)
        {
            if (!allowHistoricalData && scheduledDate < DateTime.UtcNow)
                throw new ArgumentException("Scheduled date cannot be in the past", nameof(scheduledDate));

            ScheduledDate = scheduledDate;
            Status = ExamRequestStatus.Scheduled;
            UpdateTimestamp();
        }

        public void StartExam()
        {
            if (Status != ExamRequestStatus.Scheduled)
                throw new InvalidOperationException("Exam must be scheduled before starting");

            Status = ExamRequestStatus.InProgress;
            UpdateTimestamp();
        }

        public void Complete(string results)
        {
            if (string.IsNullOrWhiteSpace(results))
                throw new ArgumentException("Results cannot be empty", nameof(results));

            Results = results.Trim();
            CompletedDate = DateTime.UtcNow;
            Status = ExamRequestStatus.Completed;
            UpdateTimestamp();
        }

        public void Cancel()
        {
            if (Status == ExamRequestStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed exam");

            Status = ExamRequestStatus.Cancelled;
            UpdateTimestamp();
        }
    }

    public enum ExamType
    {
        Laboratory,      // Laboratorial
        Imaging,         // Imagem (Raio-X, Tomografia, etc)
        Cardiac,         // Cardíaco (ECG, Ecocardiograma, etc)
        Endoscopy,       // Endoscopia
        Biopsy,          // Biópsia
        Ultrasound,      // Ultrassom
        Other            // Outros
    }

    public enum ExamUrgency
    {
        Routine,         // Rotina
        Urgent,          // Urgente
        Emergency        // Emergência
    }

    public enum ExamRequestStatus
    {
        Pending,         // Pendente
        Scheduled,       // Agendado
        InProgress,      // Em andamento
        Completed,       // Concluído
        Cancelled        // Cancelado
    }
}
