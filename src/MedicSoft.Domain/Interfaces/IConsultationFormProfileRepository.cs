using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Interfaces
{
    public interface IConsultationFormProfileRepository : IRepository<ConsultationFormProfile>
    {
        Task<IEnumerable<ConsultationFormProfile>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId);
        Task<IEnumerable<ConsultationFormProfile>> GetActiveProfilesAsync(string tenantId);
        Task<IEnumerable<ConsultationFormProfile>> GetSystemDefaultProfilesAsync(string tenantId);
    }
}
