using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum TriagePriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4,
        Emergency = 5
    }

    public enum QueueStatus
    {
        Waiting = 1,
        Called = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5
    }

    public class WaitingQueueEntry : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid ClinicId { get; private set; }
        public Guid PatientId { get; private set; }
        public int Position { get; private set; }
        public TriagePriority Priority { get; private set; }
        public QueueStatus Status { get; private set; }
        public DateTime CheckInTime { get; private set; }
        public DateTime? CalledTime { get; private set; }
        public DateTime? CompletedTime { get; private set; }
        public string? TriageNotes { get; private set; }
        public int EstimatedWaitTimeMinutes { get; private set; }

        // Navigation properties
        public Appointment Appointment { get; private set; } = null!;
        public Patient Patient { get; private set; } = null!;
        public Clinic Clinic { get; private set; } = null!;

        private WaitingQueueEntry()
        {
            // EF Core constructor
        }

        public WaitingQueueEntry(
            Guid appointmentId, 
            Guid clinicId, 
            Guid patientId,
            TriagePriority priority,
            string tenantId,
            string? triageNotes = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("O ID do agendamento não pode estar vazio", nameof(appointmentId));

            if (clinicId == Guid.Empty)
                throw new ArgumentException("O ID da clínica não pode estar vazio", nameof(clinicId));

            if (patientId == Guid.Empty)
                throw new ArgumentException("O ID do paciente não pode estar vazio", nameof(patientId));

            AppointmentId = appointmentId;
            ClinicId = clinicId;
            PatientId = patientId;
            Priority = priority;
            Status = QueueStatus.Waiting;
            CheckInTime = DateTime.UtcNow;
            TriageNotes = triageNotes?.Trim();
            Position = 0; // Will be set by the queue service
            EstimatedWaitTimeMinutes = 0;
        }

        public void UpdatePriority(TriagePriority newPriority, string? triageNotes = null)
        {
            if (Status != QueueStatus.Waiting)
                throw new InvalidOperationException("Só é possível atualizar a prioridade de entradas que estão aguardando");

            Priority = newPriority;
            if (!string.IsNullOrWhiteSpace(triageNotes))
                TriageNotes = triageNotes.Trim();
            
            UpdateTimestamp();
        }

        public void UpdatePosition(int position)
        {
            if (position < 0)
                throw new ArgumentException("A posição não pode ser negativa", nameof(position));

            Position = position;
            UpdateTimestamp();
        }

        public void UpdateEstimatedWaitTime(int minutes)
        {
            if (minutes < 0)
                throw new ArgumentException("O tempo estimado não pode ser negativo", nameof(minutes));

            EstimatedWaitTimeMinutes = minutes;
            UpdateTimestamp();
        }

        public void Call()
        {
            if (Status != QueueStatus.Waiting)
                throw new InvalidOperationException("Só é possível chamar entradas que estão aguardando");

            Status = QueueStatus.Called;
            CalledTime = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void StartService()
        {
            if (Status != QueueStatus.Called)
                throw new InvalidOperationException("Só é possível iniciar atendimento de entradas que foram chamadas");

            Status = QueueStatus.InProgress;
            UpdateTimestamp();
        }

        public void Complete()
        {
            if (Status != QueueStatus.InProgress)
                throw new InvalidOperationException("Só é possível completar entradas que estão em atendimento");

            Status = QueueStatus.Completed;
            CompletedTime = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Cancel()
        {
            if (Status == QueueStatus.Completed)
                throw new InvalidOperationException("Não é possível cancelar entradas já completadas");

            Status = QueueStatus.Cancelled;
            UpdateTimestamp();
        }

        public TimeSpan GetWaitingTime()
        {
            var endTime = CalledTime ?? DateTime.UtcNow;
            return endTime - CheckInTime;
        }

        public bool IsActive()
        {
            return Status == QueueStatus.Waiting || 
                   Status == QueueStatus.Called || 
                   Status == QueueStatus.InProgress;
        }
    }
}
