using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissGlosaRepository : BaseRepository<TissGlosa>, ITissGlosaRepository
    {
        public TissGlosaRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TissGlosa>> GetByGuideIdAsync(Guid guideId, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => g.GuideId == guideId && g.TenantId == tenantId)
                .OrderByDescending(g => g.DataGlosa)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGlosa>> GetByGuideNumberAsync(string guideNumber, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => g.NumeroGuia == guideNumber && g.TenantId == tenantId)
                .OrderByDescending(g => g.DataGlosa)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGlosa>> GetByStatusAsync(StatusGlosa status, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => g.Status == status && g.TenantId == tenantId)
                .OrderByDescending(g => g.DataGlosa)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGlosa>> GetByTipoAsync(TipoGlosa tipo, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => g.Tipo == tipo && g.TenantId == tenantId)
                .OrderByDescending(g => g.DataGlosa)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGlosa>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => g.DataGlosa >= startDate && g.DataGlosa <= endDate && g.TenantId == tenantId)
                .OrderByDescending(g => g.DataGlosa)
                .ToListAsync();
        }

        public async Task<TissGlosa?> GetWithRecursosAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Include(g => g.Recursos)
                .Where(g => g.Id == id && g.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TissGlosa>> GetPendingRecursosAsync(string tenantId)
        {
            return await _dbSet
                .Include(g => g.Guide)
                .Where(g => (g.Status == StatusGlosa.Nova || g.Status == StatusGlosa.EmAnalise) && g.TenantId == tenantId)
                .OrderBy(g => g.DataGlosa)
                .ToListAsync();
        }
    }
}
