using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISoapRecordRepository : IRepository<SoapRecord>
    {
        Task<SoapRecord?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<SoapRecord>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<SoapRecord>> GetByDoctorIdAsync(Guid doctorId, string tenantId);
    }
}
