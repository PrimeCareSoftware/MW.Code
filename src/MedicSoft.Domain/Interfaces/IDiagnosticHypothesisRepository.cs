using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for DiagnosticHypothesis entity (CFM 1.821 compliance)
    /// </summary>
    public interface IDiagnosticHypothesisRepository : IRepository<DiagnosticHypothesis>
    {
        /// <summary>
        /// Gets all diagnostic hypotheses for a specific medical record
        /// </summary>
        Task<IEnumerable<DiagnosticHypothesis>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets the principal diagnosis for a medical record
        /// </summary>
        Task<DiagnosticHypothesis?> GetPrincipalDiagnosisByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets all diagnoses by ICD-10 code (for statistical purposes)
        /// </summary>
        Task<IEnumerable<DiagnosticHypothesis>> GetByICD10CodeAsync(string icd10Code, string tenantId);
    }
}
