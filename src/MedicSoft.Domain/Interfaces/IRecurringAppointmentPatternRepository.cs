using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IRecurringAppointmentPatternRepository : IRepository<RecurringAppointmentPattern>
    {
        Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByClinicAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByProfessionalAsync(Guid professionalId, string tenantId);
        Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByPatientAsync(Guid patientId, string tenantId);
        Task<RecurringAppointmentPattern?> GetByIdAsync(Guid id, string tenantId);
    }
}
