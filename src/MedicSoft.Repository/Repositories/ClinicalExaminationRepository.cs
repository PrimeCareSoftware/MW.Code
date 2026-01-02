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
    /// Repository implementation for ClinicalExamination entity (CFM 1.821 compliance)
    /// </summary>
    public class ClinicalExaminationRepository : BaseRepository<ClinicalExamination>, IClinicalExaminationRepository
    {
        public ClinicalExaminationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ClinicalExamination>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(ce => ce.MedicalRecordId == medicalRecordId && ce.TenantId == tenantId)
                .OrderBy(ce => ce.CreatedAt)
                .ToListAsync();
        }

        public async Task<ClinicalExamination?> GetLatestByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(ce => ce.MedicalRecordId == medicalRecordId && ce.TenantId == tenantId)
                .OrderByDescending(ce => ce.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
