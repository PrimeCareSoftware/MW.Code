using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Interfaces
{
    public interface IGlobalDocumentTemplateRepository : IRepository<GlobalDocumentTemplate>
    {
        /// <summary>
        /// Gets all global templates by type
        /// </summary>
        Task<IEnumerable<GlobalDocumentTemplate>> GetByTypeAsync(DocumentTemplateType type, string tenantId);
        
        /// <summary>
        /// Gets all global templates by specialty
        /// </summary>
        Task<IEnumerable<GlobalDocumentTemplate>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId);
        
        /// <summary>
        /// Gets all active global templates
        /// </summary>
        Task<IEnumerable<GlobalDocumentTemplate>> GetActiveTemplatesAsync(string tenantId);
        
        /// <summary>
        /// Sets the active status of a global template
        /// </summary>
        Task SetActiveStatusAsync(Guid id, bool isActive, string tenantId);
        
        /// <summary>
        /// Checks if a global template name already exists for a specific type
        /// </summary>
        Task<bool> ExistsByNameAndTypeAsync(string name, DocumentTemplateType type, string tenantId, Guid? excludeId = null);
    }
}
