using System.Threading.Tasks;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITwoFactorAuthRepository : IRepository<TwoFactorAuth>
    {
        Task<TwoFactorAuth?> GetByUserIdAsync(string userId, string tenantId);
        Task<bool> IsEnabledForUserAsync(string userId, string tenantId);
    }
}
