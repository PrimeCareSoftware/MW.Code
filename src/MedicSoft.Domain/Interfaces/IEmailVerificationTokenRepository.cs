using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IEmailVerificationTokenRepository
    {
        Task<EmailVerificationToken?> GetByCodeAndUserIdAsync(string code, Guid userId, string tenantId);
        Task<int> CountRecentTokensAsync(Guid userId, string tenantId, TimeSpan timeWindow);
        Task AddAsync(EmailVerificationToken token);
        Task UpdateAsync(EmailVerificationToken token);
        Task DeleteExpiredTokensAsync(string tenantId);
    }
}
