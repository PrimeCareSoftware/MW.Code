using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAnamnesisResponseRepository : IRepository<AnamnesisResponse>
    {
        Task<List<AnamnesisResponse>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<AnamnesisResponse?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<List<AnamnesisResponse>> GetByDoctorIdAsync(Guid doctorId, string tenantId);
    }
}
