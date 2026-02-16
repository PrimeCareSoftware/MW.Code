using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAccessProfileRepository
    {
        Task<AccessProfile?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<AccessProfile>> GetAllAsync(string tenantId);
        Task<IEnumerable<AccessProfile>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<AccessProfile>> GetDefaultProfilesAsync(string tenantId);
        Task<AccessProfile?> GetByNameAsync(string name, Guid clinicId, string tenantId);
        IQueryable<AccessProfile> GetAllQueryable();
        Task AddAsync(AccessProfile profile);
        Task UpdateAsync(AccessProfile profile);
        Task DeleteAsync(Guid id, string tenantId);
        Task<bool> ExistsAsync(Guid id, string tenantId);
        Task<bool> IsProfileInUseAsync(Guid profileId, string tenantId);
    }
}
