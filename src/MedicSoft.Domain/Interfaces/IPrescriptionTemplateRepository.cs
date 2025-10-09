using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPrescriptionTemplateRepository : IRepository<PrescriptionTemplate>
    {
        Task<IEnumerable<PrescriptionTemplate>> GetByTenantAsync(string tenantId);
        Task<IEnumerable<PrescriptionTemplate>> GetActiveByCategoryAsync(string category, string tenantId);
        Task<IEnumerable<PrescriptionTemplate>> SearchByNameAsync(string name, string tenantId);
    }
}
