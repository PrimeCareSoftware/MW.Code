using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ILoginAttemptRepository : IRepository<LoginAttempt>
    {
        Task<IEnumerable<LoginAttempt>> GetRecentFailedAttemptsAsync(string username, string tenantId, int minutes = 30);
        Task<IEnumerable<LoginAttempt>> GetRecentAttemptsByIpAsync(string ipAddress, string tenantId, int minutes = 30);
        Task<int> GetFailedAttemptsCountAsync(string username, string tenantId, int minutes = 30);
    }
}
