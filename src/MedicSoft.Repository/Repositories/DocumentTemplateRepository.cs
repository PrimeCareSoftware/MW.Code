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
    public class DocumentTemplateRepository : BaseRepository<DocumentTemplate>, IDocumentTemplateRepository
    {
        public DocumentTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(x => x.Specialty == specialty && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1) // System templates first
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetSystemTemplatesBySpecialtyAsync(ProfessionalSpecialty specialty)
        {
            return await _dbSet
                .Where(x => x.Specialty == specialty && x.IsSystem)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId && x.ClinicId == clinicId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetByTypeAsync(DocumentTemplateType type, string tenantId)
        {
            return await _dbSet
                .Where(x => x.Type == type && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetActiveTemplatesAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(x => x.Specialty == specialty && x.IsActive && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
    }
}
