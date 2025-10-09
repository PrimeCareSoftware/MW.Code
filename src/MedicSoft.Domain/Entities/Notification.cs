using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a notification sent to a patient (SMS, WhatsApp, Email).
    /// Tracks notification history and delivery status.
    /// </summary>
    public class Notification : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid? AppointmentId { get; private set; }
        public NotificationType Type { get; private set; }
        public NotificationChannel Channel { get; private set; }
        public string Recipient { get; private set; }
        public string Message { get; private set; }
        public NotificationStatus Status { get; private set; }
        public DateTime? SentAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int RetryCount { get; private set; }

        // Navigation properties
        public Patient? Patient { get; private set; }
        public Appointment? Appointment { get; private set; }

        private Notification()
        {
            // EF Constructor
            Recipient = null!;
            Message = null!;
        }

        public Notification(Guid patientId, NotificationType type, NotificationChannel channel,
            string recipient, string message, string tenantId, Guid? appointmentId = null) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Recipient cannot be empty", nameof(recipient));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));

            PatientId = patientId;
            AppointmentId = appointmentId;
            Type = type;
            Channel = channel;
            Recipient = recipient.Trim();
            Message = message.Trim();
            Status = NotificationStatus.Pending;
            RetryCount = 0;
        }

        public void MarkAsSent()
        {
            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsDelivered()
        {
            Status = NotificationStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsRead()
        {
            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsFailed(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

            Status = NotificationStatus.Failed;
            ErrorMessage = errorMessage.Trim();
            UpdateTimestamp();
        }

        public void IncrementRetryCount()
        {
            RetryCount++;
            Status = NotificationStatus.Pending;
            UpdateTimestamp();
        }

        public bool CanRetry()
        {
            return Status == NotificationStatus.Failed && RetryCount < 3;
        }
    }

    public enum NotificationType
    {
        AppointmentReminder,     // Lembrete de consulta
        AppointmentConfirmation, // Confirmação de agendamento
        AppointmentCancellation, // Cancelamento de consulta
        AppointmentRescheduled,  // Reagendamento
        PaymentReminder,         // Lembrete de pagamento
        PrescriptionReady,       // Receita pronta
        ExamResults,            // Resultados de exame
        General                 // Notificação geral
    }

    public enum NotificationChannel
    {
        SMS,        // SMS
        WhatsApp,   // WhatsApp
        Email,      // Email
        Push        // Push notification
    }

    public enum NotificationStatus
    {
        Pending,    // Pendente de envio
        Sent,       // Enviado
        Delivered,  // Entregue
        Read,       // Lido
        Failed      // Falhou
    }
}
