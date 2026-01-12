using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AuthorizationRequestRepository : BaseRepository<AuthorizationRequest>, IAuthorizationRequestRepository
    {
        public AuthorizationRequestRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<AuthorizationRequest?> GetByRequestNumberAsync(string requestNumber, string tenantId)
        {
            return await _dbSet
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .Where(ar => ar.RequestNumber == requestNumber && ar.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(ar => ar.PatientHealthInsurance)
                .Where(ar => ar.PatientId == patientId && ar.TenantId == tenantId)
                .OrderByDescending(ar => ar.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetByStatusAsync(AuthorizationStatus status, string tenantId)
        {
            return await _dbSet
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .Where(ar => ar.Status == status && ar.TenantId == tenantId)
                .OrderBy(ar => ar.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetPendingAuthorizationsAsync(string tenantId)
        {
            return await GetByStatusAsync(AuthorizationStatus.Pending, tenantId);
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetExpiredAuthorizationsAsync(string tenantId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .Where(ar => ar.Status == AuthorizationStatus.Approved && 
                            ar.ExpirationDate.HasValue && 
                            ar.ExpirationDate.Value < now && 
                            ar.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<AuthorizationRequest?> GetByAuthorizationNumberAsync(string authorizationNumber, string tenantId)
        {
            return await _dbSet
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .Where(ar => ar.AuthorizationNumber == authorizationNumber && ar.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }
    }
}
