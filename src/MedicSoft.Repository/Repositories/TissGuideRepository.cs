using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissGuideRepository : BaseRepository<TissGuide>, ITissGuideRepository
    {
        public TissGuideRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<TissGuide?> GetByGuideNumberAsync(string guideNumber, string tenantId)
        {
            return await _dbSet
                .Where(g => g.GuideNumber == guideNumber && g.TenantId == tenantId)
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByBatchIdAsync(Guid batchId, string tenantId)
        {
            return await _dbSet
                .Where(g => g.TissBatchId == batchId && g.TenantId == tenantId)
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .AsNoTracking()
                .OrderBy(g => g.GuideNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(g => g.AppointmentId == appointmentId && g.TenantId == tenantId)
                .Include(g => g.PatientHealthInsurance)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByStatusAsync(GuideStatus status, string tenantId)
        {
            return await _dbSet
                .Where(g => g.Status == status && g.TenantId == tenantId)
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .AsNoTracking()
                .OrderBy(g => g.ServiceDate)
                .ToListAsync();
        }

        public async Task<TissGuide?> GetWithProceduresAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(g => g.Id == id && g.TenantId == tenantId)
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .Include(g => g.Procedures)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
