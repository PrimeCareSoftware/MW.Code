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
    /// Repository implementation for DiagnosticHypothesis entity (CFM 1.821 compliance)
    /// </summary>
    public class DiagnosticHypothesisRepository : BaseRepository<DiagnosticHypothesis>, IDiagnosticHypothesisRepository
    {
        public DiagnosticHypothesisRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DiagnosticHypothesis>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(dh => dh.MedicalRecordId == medicalRecordId && dh.TenantId == tenantId)
                .OrderBy(dh => dh.Type)
                .ThenBy(dh => dh.CreatedAt)
                .ToListAsync();
        }

        public async Task<DiagnosticHypothesis?> GetPrincipalDiagnosisByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(dh => dh.MedicalRecordId == medicalRecordId 
                    && dh.TenantId == tenantId 
                    && dh.Type == DiagnosisType.Principal)
                .OrderByDescending(dh => dh.DiagnosedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DiagnosticHypothesis>> GetByICD10CodeAsync(string icd10Code, string tenantId)
        {
            return await _dbSet
                .Where(dh => dh.ICD10Code == icd10Code.ToUpperInvariant() && dh.TenantId == tenantId)
                .OrderByDescending(dh => dh.DiagnosedAt)
                .ToListAsync();
        }
    }
}
