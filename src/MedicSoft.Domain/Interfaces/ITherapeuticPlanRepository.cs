using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for TherapeuticPlan entity (CFM 1.821 compliance)
    /// </summary>
    public interface ITherapeuticPlanRepository : IRepository<TherapeuticPlan>
    {
        /// <summary>
        /// Gets all therapeutic plans for a specific medical record
        /// </summary>
        Task<IEnumerable<TherapeuticPlan>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets the most recent therapeutic plan for a medical record
        /// </summary>
        Task<TherapeuticPlan?> GetLatestByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets all therapeutic plans with return dates in a date range
        /// </summary>
        Task<IEnumerable<TherapeuticPlan>> GetByReturnDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
    }
}
