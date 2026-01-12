using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissBatchRepository : BaseRepository<TissBatch>, ITissBatchRepository
    {
        public TissBatchRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<TissBatch?> GetByBatchNumberAsync(string batchNumber, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Operator)
                .Include(b => b.Clinic)
                .Where(b => b.BatchNumber == batchNumber && b.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TissBatch>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Operator)
                .Where(b => b.ClinicId == clinicId && b.TenantId == tenantId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissBatch>> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Where(b => b.OperatorId == operatorId && b.TenantId == tenantId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissBatch>> GetByStatusAsync(BatchStatus status, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Operator)
                .Include(b => b.Clinic)
                .Where(b => b.Status == status && b.TenantId == tenantId)
                .OrderBy(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<TissBatch?> GetWithGuidesAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Operator)
                .Include(b => b.Clinic)
                .Include(b => b.Guides)
                    .ThenInclude(g => g.Procedures)
                .Where(b => b.Id == id && b.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }
    }
}
