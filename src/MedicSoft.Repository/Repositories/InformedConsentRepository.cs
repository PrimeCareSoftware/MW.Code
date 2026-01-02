using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for InformedConsent entity (CFM 1.821 compliance)
    /// </summary>
    public class InformedConsentRepository : BaseRepository<InformedConsent>, IInformedConsentRepository
    {
        public InformedConsentRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InformedConsent>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(ic => ic.MedicalRecordId == medicalRecordId && ic.TenantId == tenantId)
                .OrderBy(ic => ic.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InformedConsent>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(ic => ic.PatientId == patientId && ic.TenantId == tenantId)
                .OrderByDescending(ic => ic.CreatedAt)
                .ToListAsync();
        }

        public async Task<InformedConsent?> GetActiveConsentByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(ic => ic.MedicalRecordId == medicalRecordId 
                    && ic.TenantId == tenantId 
                    && ic.IsAccepted)
                .OrderByDescending(ic => ic.AcceptedAt)
                .FirstOrDefaultAsync();
        }
    }
}
