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
                .Where(ar => ar.RequestNumber == requestNumber && ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(ar => ar.PatientId == patientId && ar.TenantId == tenantId)
                .Include(ar => ar.PatientHealthInsurance)
                .AsNoTracking()
                .OrderByDescending(ar => ar.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetByStatusAsync(AuthorizationStatus status, string tenantId)
        {
            return await _dbSet
                .Where(ar => ar.Status == status && ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .AsNoTracking()
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
                .Where(ar => ar.Status == AuthorizationStatus.Approved && 
                            ar.ExpirationDate.HasValue && 
                            ar.ExpirationDate.Value < now && 
                            ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AuthorizationRequest?> GetByAuthorizationNumberAsync(string authorizationNumber, string tenantId)
        {
            return await _dbSet
                .Where(ar => ar.AuthorizationNumber == authorizationNumber && ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<AuthorizationRequest?> GetByIdWithDetailsAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(ar => ar.Id == id && ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                    .ThenInclude(phi => phi!.HealthInsurancePlan)
                        .ThenInclude(plan => plan!.Operator)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AuthorizationRequest>> GetAllWithDetailsAsync(string tenantId)
        {
            return await _dbSet
                .Where(ar => ar.TenantId == tenantId)
                .Include(ar => ar.Patient)
                .Include(ar => ar.PatientHealthInsurance)
                    .ThenInclude(phi => phi!.HealthInsurancePlan)
                        .ThenInclude(plan => plan!.Operator)
                .AsNoTracking()
                .OrderByDescending(ar => ar.RequestDate)
                .ToListAsync();
        }
    }
}
