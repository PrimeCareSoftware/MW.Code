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
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .Where(g => g.GuideNumber == guideNumber && g.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByBatchIdAsync(Guid batchId, string tenantId)
        {
            return await _dbSet
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .Where(g => g.TissBatchId == batchId && g.TenantId == tenantId)
                .OrderBy(g => g.GuideNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Include(g => g.PatientHealthInsurance)
                .Where(g => g.AppointmentId == appointmentId && g.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TissGuide>> GetByStatusAsync(GuideStatus status, string tenantId)
        {
            return await _dbSet
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .Where(g => g.Status == status && g.TenantId == tenantId)
                .OrderBy(g => g.ServiceDate)
                .ToListAsync();
        }

        public async Task<TissGuide?> GetWithProceduresAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(g => g.PatientHealthInsurance)
                .Include(g => g.Appointment)
                .Include(g => g.Procedures)
                .Where(g => g.Id == id && g.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }
    }
}
