using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ClinicalExamination entity (CFM 1.821 compliance)
    /// </summary>
    public interface IClinicalExaminationRepository : IRepository<ClinicalExamination>
    {
        /// <summary>
        /// Gets all clinical examinations for a specific medical record
        /// </summary>
        Task<IEnumerable<ClinicalExamination>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets the most recent clinical examination for a medical record
        /// </summary>
        Task<ClinicalExamination?> GetLatestByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }
}
