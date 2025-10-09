using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMedicalRecordTemplateRepository : IRepository<MedicalRecordTemplate>
    {
        Task<IEnumerable<MedicalRecordTemplate>> GetByTenantAsync(string tenantId);
        Task<IEnumerable<MedicalRecordTemplate>> GetActiveByCategoryAsync(string category, string tenantId);
        Task<IEnumerable<MedicalRecordTemplate>> SearchByNameAsync(string name, string tenantId);
    }
}
