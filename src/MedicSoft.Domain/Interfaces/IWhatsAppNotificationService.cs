using System;
using System.Threading.Tasks;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface for WhatsApp notification service.
    /// Implementations should integrate with WhatsApp Business API.
    /// </summary>
    public interface IWhatsAppNotificationService
    {
        Task<bool> SendMessageAsync(string phoneNumber, string message);
        Task<bool> SendAppointmentReminderAsync(Guid appointmentId, string phoneNumber, DateTime appointmentDate, string clinicName);
        Task<bool> SendAppointmentConfirmationAsync(Guid appointmentId, string phoneNumber, DateTime appointmentDate, string clinicName);
    }
}
