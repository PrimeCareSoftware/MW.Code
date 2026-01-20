using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IConsultationFormConfigurationRepository : IRepository<ConsultationFormConfiguration>
    {
        Task<ConsultationFormConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<ConsultationFormConfiguration>> GetByProfileIdAsync(Guid profileId, string tenantId);
        Task<ConsultationFormConfiguration?> GetActiveConfigurationByClinicIdAsync(Guid clinicId, string tenantId);
    }
}
