using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<MedicalRecord?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(Guid patientId, string tenantId);
    }
}
