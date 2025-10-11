using System;
using System.Threading.Tasks;

namespace MedicSoft.WhatsAppAgent.Interfaces
{
    /// <summary>
    /// Interface for appointment management operations
    /// Integrates with existing MedicSoft appointment API
    /// </summary>
    public interface IAppointmentManagementService
    {
        /// <summary>
        /// Get available appointment slots for a date
        /// </summary>
        Task<dynamic> GetAvailableSlotsAsync(string tenantId, DateTime date, Guid? doctorId = null);
        
        /// <summary>
        /// Create a new appointment
        /// </summary>
        Task<dynamic> CreateAppointmentAsync(string tenantId, dynamic appointmentData);
        
        /// <summary>
        /// Reschedule an existing appointment
        /// </summary>
        Task<bool> RescheduleAppointmentAsync(string tenantId, Guid appointmentId, DateTime newDateTime);
        
        /// <summary>
        /// Cancel an appointment
        /// </summary>
        Task<bool> CancelAppointmentAsync(string tenantId, Guid appointmentId, string reason);
        
        /// <summary>
        /// Get patient appointments
        /// </summary>
        Task<dynamic> GetPatientAppointmentsAsync(string tenantId, string patientPhone);
    }
}
