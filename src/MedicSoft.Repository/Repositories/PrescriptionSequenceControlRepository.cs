using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PrescriptionSequenceControlRepository : BaseRepository<PrescriptionSequenceControl>, IPrescriptionSequenceControlRepository
    {
        public PrescriptionSequenceControlRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<PrescriptionSequenceControl?> GetByTypeAsync(PrescriptionType type, string tenantId)
        {
            var currentYear = DateTime.UtcNow.Year;
            return await _dbSet
                .Where(psc => psc.Type == type && psc.Year == currentYear && psc.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<PrescriptionSequenceControl> GetOrCreateByTypeAsync(PrescriptionType type, string tenantId, string? prefix = null)
        {
            var currentYear = DateTime.UtcNow.Year;
            var control = await _dbSet
                .Where(psc => psc.Type == type && psc.Year == currentYear && psc.TenantId == tenantId)
                .FirstOrDefaultAsync();
            
            if (control == null)
            {
                control = new PrescriptionSequenceControl(type, tenantId, prefix);
                await AddAsync(control);
            }
            
            return control;
        }

        public async Task<string> GenerateNextSequenceAsync(PrescriptionType type, string tenantId, string? prefix = null)
        {
            var control = await GetOrCreateByTypeAsync(type, tenantId, prefix);
            
            var sequenceNumber = control.GenerateNext();
            await UpdateAsync(control);
            
            return sequenceNumber;
        }

        public async Task<string> PreviewNextSequenceAsync(PrescriptionType type, string tenantId)
        {
            var control = await GetByTypeAsync(type, tenantId);
            
            if (control == null)
            {
                // If no control exists yet, return the first sequence number
                var tempControl = new PrescriptionSequenceControl(type, tenantId);
                return tempControl.PreviewNext();
            }
            
            return control.PreviewNext();
        }

        public async Task<PrescriptionSequenceControl?> GetByTypeAndYearAsync(PrescriptionType type, int year, string tenantId)
        {
            return await _dbSet
                .Where(psc => psc.Type == type && psc.Year == year && psc.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PrescriptionSequenceControl>> GetAllForYearAsync(int year, string tenantId)
        {
            return await _dbSet
                .Where(psc => psc.Year == year && psc.TenantId == tenantId)
                .OrderBy(psc => psc.Type)
                .ToListAsync();
        }
    }
}
