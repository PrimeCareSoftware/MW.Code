using System;
using System.Threading.Tasks;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface for SMS notification service.
    /// Implementations should integrate with SMS providers (Twilio, AWS SNS, etc.)
    /// </summary>
    public interface ISmsNotificationService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendAppointmentReminderAsync(Guid appointmentId, string phoneNumber, DateTime appointmentDate);
    }
}
