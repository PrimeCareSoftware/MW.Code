using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissRecursoGlosaRepository : BaseRepository<TissRecursoGlosa>, ITissRecursoGlosaRepository
    {
        public TissRecursoGlosaRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TissRecursoGlosa>> GetByGlosaIdAsync(Guid glosaId, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Glosa)
                .Where(r => r.GlosaId == glosaId && r.TenantId == tenantId)
                .OrderByDescending(r => r.DataEnvio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissRecursoGlosa>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Glosa)
                .Where(r => r.DataEnvio >= startDate && r.DataEnvio <= endDate && r.TenantId == tenantId)
                .OrderByDescending(r => r.DataEnvio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissRecursoGlosa>> GetPendingResponseAsync(string tenantId)
        {
            return await _dbSet
                .Include(r => r.Glosa)
                .Where(r => !r.DataResposta.HasValue && r.TenantId == tenantId)
                .OrderBy(r => r.DataEnvio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissRecursoGlosa>> GetByResultadoAsync(ResultadoRecurso resultado, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Glosa)
                .Where(r => r.Resultado == resultado && r.TenantId == tenantId)
                .OrderByDescending(r => r.DataResposta)
                .ToListAsync();
        }
    }
}
