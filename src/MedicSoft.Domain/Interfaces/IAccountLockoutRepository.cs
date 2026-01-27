using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAccountLockoutRepository : IRepository<AccountLockout>
    {
        Task<AccountLockout?> GetActiveLockedAccountAsync(string userId, string tenantId);
        Task<int> GetLockoutCountAsync(string userId, string tenantId);
    }
}
