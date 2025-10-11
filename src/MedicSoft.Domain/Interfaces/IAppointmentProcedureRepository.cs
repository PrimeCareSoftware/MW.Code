using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAppointmentProcedureRepository : IRepository<AppointmentProcedure>
    {
        Task<IEnumerable<AppointmentProcedure>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<AppointmentProcedure>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<decimal> GetAppointmentTotalAsync(Guid appointmentId, string tenantId);
    }
}
