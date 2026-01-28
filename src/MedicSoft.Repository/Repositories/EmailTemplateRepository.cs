using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class EmailTemplateRepository : BaseRepository<EmailTemplate>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<EmailTemplate?> GetByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(et => et.Name == name && et.TenantId == tenantId);
        }

        public async Task<IEnumerable<EmailTemplate>> GetAllForTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(et => et.TenantId == tenantId)
                .OrderBy(et => et.Name)
                .ToListAsync();
        }

        public async Task<bool> NameExistsAsync(string name, string tenantId)
        {
            return await _dbSet
                .AnyAsync(et => et.Name == name && et.TenantId == tenantId);
        }
    }
}
