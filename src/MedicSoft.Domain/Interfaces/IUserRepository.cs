using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<object?> GetByUsernameAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username, string tenantId);
        Task<User?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<User>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
