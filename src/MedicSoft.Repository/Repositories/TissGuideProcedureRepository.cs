using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissGuideProcedureRepository : BaseRepository<TissGuideProcedure>, ITissGuideProcedureRepository
    {
        public TissGuideProcedureRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TissGuideProcedure>> GetByGuideIdAsync(Guid guideId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.TissGuideId == guideId && p.TenantId == tenantId)
                .OrderBy(p => p.ProcedureCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGuideProcedure>> GetByProcedureCodeAsync(string procedureCode, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ProcedureCode == procedureCode && p.TenantId == tenantId)
                .ToListAsync();
        }
    }
}
