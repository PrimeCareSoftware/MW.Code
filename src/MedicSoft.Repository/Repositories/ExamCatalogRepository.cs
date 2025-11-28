using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ExamCatalogRepository : BaseRepository<ExamCatalog>, IExamCatalogRepository
    {
        public ExamCatalogRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ExamCatalog?> GetByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Name == name && e.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExamCatalog>> SearchByNameAsync(string searchTerm, string tenantId)
        {
            // Use EF.Functions.ILike for PostgreSQL or EF.Functions.Like for SQL Server
            // This enables case-insensitive search with potential index usage
            return await _dbSet
                .Where(e => (EF.Functions.Like(e.Name, $"%{searchTerm}%") ||
                           (e.Synonyms != null && EF.Functions.Like(e.Synonyms, $"%{searchTerm}%"))) &&
                           e.TenantId == tenantId && e.IsActive)
                .OrderBy(e => e.Name)
                .Take(20) // Limit results for better performance
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamCatalog>> GetByExamTypeAsync(ExamType examType, string tenantId)
        {
            return await _dbSet
                .Where(e => e.ExamType == examType && e.TenantId == tenantId && e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamCatalog>> GetByCategoryAsync(string category, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Category == category && e.TenantId == tenantId && e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamCatalog>> GetActiveAsync(string tenantId)
        {
            return await _dbSet
                .Where(e => e.IsActive && e.TenantId == tenantId)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(e => e.Name == name && e.TenantId == tenantId);

            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}
