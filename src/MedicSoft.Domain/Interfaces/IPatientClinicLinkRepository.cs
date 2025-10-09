using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPatientClinicLinkRepository : IRepository<PatientClinicLink>
    {
        Task<IEnumerable<PatientClinicLink>> GetPatientClinicsAsync(Guid patientId);
        Task<IEnumerable<PatientClinicLink>> GetClinicPatientsAsync(Guid clinicId, string tenantId);
        Task<PatientClinicLink?> GetLinkAsync(Guid patientId, Guid clinicId, string tenantId);
        Task<bool> IsPatientLinkedToClinicAsync(Guid patientId, Guid clinicId, string tenantId);
    }
}
