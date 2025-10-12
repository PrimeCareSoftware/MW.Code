using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IOwnerRepository
    {
        Task<Owner?> GetByUsernameAsync(string username, string tenantId);
        Task<Owner?> GetByIdAsync(Guid id, string tenantId);
        Task<Owner?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Owner>> GetAllAsync(string tenantId);
        Task AddAsync(Owner owner);
        Task UpdateAsync(Owner owner);
        Task<bool> ExistsByUsernameAsync(string username, string tenantId);
        Task<bool> ExistsByEmailAsync(string email, string tenantId);
    }
}
