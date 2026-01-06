using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMedicationRepository : IRepository<Medication>
    {
        Task<Medication?> GetByNameAsync(string name, string tenantId);
        Task<IEnumerable<Medication>> SearchByNameAsync(string searchTerm, string tenantId);
        Task<IEnumerable<Medication>> GetByCategoryAsync(MedicationCategory category, string tenantId);
        Task<IEnumerable<Medication>> GetActiveAsync(string tenantId);
        Task<IEnumerable<Medication>> GetByActiveIngredientAsync(string activeIngredient, string tenantId);
        Task<bool> IsNameUniqueAsync(string name, string tenantId, Guid? excludeId = null);
        
        /// <summary>
        /// Gets all controlled substances.
        /// </summary>
        Task<IEnumerable<Medication>> GetControlledSubstancesAsync(string tenantId);
        
        /// <summary>
        /// Gets medications by controlled substance list classification.
        /// </summary>
        Task<IEnumerable<Medication>> GetByControlledListAsync(ControlledSubstanceList controlledList, string tenantId);
        
        /// <summary>
        /// Gets medications that require special prescription type (controlled).
        /// </summary>
        Task<IEnumerable<Medication>> GetRequiringSpecialPrescriptionAsync(string tenantId);
    }
}
