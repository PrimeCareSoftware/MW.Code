using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class GlobalDocumentTemplateRepository : BaseRepository<GlobalDocumentTemplate>, IGlobalDocumentTemplateRepository
    {
        public GlobalDocumentTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<GlobalDocumentTemplate>> GetByTypeAsync(DocumentTemplateType type, string tenantId)
        {
            return await _dbSet
                .Where(x => x.Type == type && x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<GlobalDocumentTemplate>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(x => x.Specialty == specialty && x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<GlobalDocumentTemplate>> GetActiveTemplatesAsync(string tenantId)
        {
            return await _dbSet
                .Where(x => x.IsActive && x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task SetActiveStatusAsync(Guid id, bool isActive, string tenantId)
        {
            var template = await GetByIdAsync(id, tenantId);
            if (template == null)
            {
                throw new InvalidOperationException($"Global template with ID {id} not found");
            }
            
            template.SetActiveStatus(isActive);
            await UpdateAsync(template);
        }
        
        public async Task<bool> ExistsByNameAndTypeAsync(string name, DocumentTemplateType type, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(x => x.Name == name && x.Type == type && x.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}
