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
    }
}
