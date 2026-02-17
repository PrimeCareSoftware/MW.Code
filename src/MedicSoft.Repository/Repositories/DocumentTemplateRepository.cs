using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private static readonly SemaphoreSlim GlobalTemplateIdRepairLock = new(1, 1);
        private static bool _globalTemplateIdChecked;

        public DocumentTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<DocumentTemplate>> GetAllAsync(string tenantId)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await base.GetAllAsync(tenantId);
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await _dbSet
                .Where(x => x.Specialty == specialty && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1) // System templates first
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetSystemTemplatesBySpecialtyAsync(ProfessionalSpecialty specialty)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await _dbSet
                .Where(x => x.Specialty == specialty && x.IsSystem)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await _dbSet
                .Where(x => x.TenantId == tenantId && x.ClinicId == clinicId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetByTypeAsync(DocumentTemplateType type, string tenantId)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await _dbSet
                .Where(x => x.Type == type && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DocumentTemplate>> GetActiveTemplatesAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            await EnsureGlobalTemplateIdColumnAsync();
            return await _dbSet
                .Where(x => x.Specialty == specialty && x.IsActive && (x.TenantId == tenantId || x.IsSystem))
                .OrderBy(x => x.IsSystem ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        private async Task EnsureGlobalTemplateIdColumnAsync()
        {
            if (_globalTemplateIdChecked)
                return;

            await GlobalTemplateIdRepairLock.WaitAsync();
            try
            {
                if (_globalTemplateIdChecked)
                    return;

                await _context.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE \"DocumentTemplates\" ADD COLUMN IF NOT EXISTS \"GlobalTemplateId\" uuid NULL;");
                await _context.Database.ExecuteSqlRawAsync(
                    "CREATE INDEX IF NOT EXISTS \"ix_documenttemplates_globaltemplateid\" ON \"DocumentTemplates\" (\"GlobalTemplateId\");");

                _globalTemplateIdChecked = true;
            }
            finally
            {
                GlobalTemplateIdRepairLock.Release();
            }
        }
    }
}
