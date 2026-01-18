using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PatientHealthInsuranceRepository : BaseRepository<PatientHealthInsurance>, IPatientHealthInsuranceRepository
    {
        public PatientHealthInsuranceRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PatientHealthInsurance>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(phi => phi.HealthInsurancePlan)
                .Where(phi => phi.PatientId == patientId && phi.TenantId == tenantId)
                .OrderByDescending(phi => phi.IsActive)
                .ThenBy(phi => phi.ValidFrom)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientHealthInsurance>> GetActiveByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(phi => phi.HealthInsurancePlan)
                .Where(phi => phi.PatientId == patientId && phi.IsActive && phi.TenantId == tenantId)
                .OrderBy(phi => phi.ValidFrom)
                .ToListAsync();
        }

        public async Task<PatientHealthInsurance?> GetByCardNumberAsync(string cardNumber, string tenantId)
        {
            return await _dbSet
                .Include(phi => phi.HealthInsurancePlan)
                .Include(phi => phi.Patient)
                .Where(phi => phi.CardNumber == cardNumber && phi.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PatientHealthInsurance>> GetByPlanIdAsync(Guid planId, string tenantId)
        {
            return await _dbSet
                .Include(phi => phi.Patient)
                .Where(phi => phi.HealthInsurancePlanId == planId && phi.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<bool> IsCardNumberUniqueAsync(string cardNumber, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(phi => phi.CardNumber == cardNumber && phi.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(phi => phi.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<PatientHealthInsurance?> GetByIdWithDetailsAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(phi => phi.Patient)
                .Include(phi => phi.HealthInsurancePlan)
                    .ThenInclude(plan => plan!.Operator)
                .FirstOrDefaultAsync(phi => phi.Id == id && phi.TenantId == tenantId);
        }
    }
}
