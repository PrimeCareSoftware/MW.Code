using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Interfaces
{
    public interface IDocumentTemplateRepository : IRepository<DocumentTemplate>
    {
        /// <summary>
        /// Gets all templates for a specific specialty
        /// </summary>
        Task<IEnumerable<DocumentTemplate>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId);
        
        /// <summary>
        /// Gets all system templates for a specific specialty
        /// </summary>
        Task<IEnumerable<DocumentTemplate>> GetSystemTemplatesBySpecialtyAsync(ProfessionalSpecialty specialty);
        
        /// <summary>
        /// Gets all clinic-specific templates
        /// </summary>
        Task<IEnumerable<DocumentTemplate>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        
        /// <summary>
        /// Gets templates by type
        /// </summary>
        Task<IEnumerable<DocumentTemplate>> GetByTypeAsync(DocumentTemplateType type, string tenantId);
        
        /// <summary>
        /// Gets active templates only
        /// </summary>
        Task<IEnumerable<DocumentTemplate>> GetActiveTemplatesAsync(ProfessionalSpecialty specialty, string tenantId);
    }
}
