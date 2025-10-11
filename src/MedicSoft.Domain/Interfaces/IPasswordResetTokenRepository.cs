using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token, string tenantId);
        Task<PasswordResetToken?> GetActiveByUserIdAsync(Guid userId, string tenantId);
        Task AddAsync(PasswordResetToken token);
        Task UpdateAsync(PasswordResetToken token);
        Task InvalidateAllByUserIdAsync(Guid userId, string tenantId);
    }
}
