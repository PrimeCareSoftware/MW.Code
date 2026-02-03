using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class DigitalPrescriptionRepository : BaseRepository<DigitalPrescription>, IDigitalPrescriptionRepository
    {
        public DigitalPrescriptionRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DigitalPrescription>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.PatientId == patientId && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.MedicalRecordId == medicalRecordId && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetByDoctorIdAsync(Guid doctorId, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.DoctorId == doctorId && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetByTypeAsync(PrescriptionType type, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.Type == type && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetRequiringSNGPCReportAsync(string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.RequiresSNGPCReport && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetUnreportedToSNGPCAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.RequiresSNGPCReport && 
                            dp.ReportedToSNGPCAt == null &&
                            dp.IssuedAt >= startDate &&
                            dp.IssuedAt <= endDate &&
                            dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderBy(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetControlledPrescriptionsByPeriodAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(dp => dp.RequiresSNGPCReport && 
                            dp.IssuedAt >= startDate &&
                            dp.IssuedAt <= endDate &&
                            dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderBy(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<DigitalPrescription?> GetByVerificationCodeAsync(string verificationCode)
        {
            return await _dbSet
                .Where(dp => dp.VerificationCode == verificationCode)
                .Include(dp => dp.Items)
                .Include(dp => dp.Patient)
                .Include(dp => dp.Doctor)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetActivePrescriptionsForPatientAsync(Guid patientId, string tenantId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(dp => dp.PatientId == patientId && 
                            dp.IsActive && 
                            dp.ExpiresAt > now &&
                            dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderByDescending(dp => dp.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescription>> GetExpiringSoonAsync(int days, string tenantId)
        {
            var now = DateTime.UtcNow;
            var targetDate = now.AddDays(days);
            
            return await _dbSet
                .Where(dp => dp.IsActive && 
                            dp.ExpiresAt > now &&
                            dp.ExpiresAt <= targetDate &&
                            dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .OrderBy(dp => dp.ExpiresAt)
                .ToListAsync();
        }

        public async Task<DigitalPrescription?> GetByIdWithItemsAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(dp => dp.Id == id && dp.TenantId == tenantId)
                .Include(dp => dp.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SequenceNumberExistsAsync(string sequenceNumber, string tenantId)
        {
            return await _dbSet
                .AnyAsync(dp => dp.SequenceNumber == sequenceNumber && dp.TenantId == tenantId);
        }
    }
}
