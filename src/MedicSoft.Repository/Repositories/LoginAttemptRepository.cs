using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class LoginAttemptRepository : BaseRepository<LoginAttempt>, ILoginAttemptRepository
    {
        public LoginAttemptRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LoginAttempt>> GetRecentFailedAttemptsAsync(string username, string tenantId, int minutes = 30)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
            
            return await _context.Set<LoginAttempt>()
                .Where(a => a.Username == username 
                    && a.TenantId == tenantId
                    && !a.WasSuccessful 
                    && a.AttemptTime > cutoffTime)
                .OrderByDescending(a => a.AttemptTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginAttempt>> GetRecentAttemptsByIpAsync(string ipAddress, string tenantId, int minutes = 30)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
            
            return await _context.Set<LoginAttempt>()
                .Where(a => a.IpAddress == ipAddress 
                    && a.TenantId == tenantId
                    && a.AttemptTime > cutoffTime)
                .OrderByDescending(a => a.AttemptTime)
                .ToListAsync();
        }

        public async Task<int> GetFailedAttemptsCountAsync(string username, string tenantId, int minutes = 30)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
            
            return await _context.Set<LoginAttempt>()
                .CountAsync(a => a.Username == username 
                    && a.TenantId == tenantId
                    && !a.WasSuccessful 
                    && a.AttemptTime > cutoffTime);
        }
    }
}
