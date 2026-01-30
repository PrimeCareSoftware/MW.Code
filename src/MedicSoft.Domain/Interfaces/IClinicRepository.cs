using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicRepository : IRepository<Clinic>
    {
        Task<Clinic?> GetByDocumentAsync(string document, string tenantId);
        Task<Clinic?> GetByDocumentAsync(string document); // Overload for registration check (document is unique across all tenants)
        Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null);
        Task<Clinic?> GetByCNPJAsync(string cnpj);
        Task<Clinic?> GetBySubdomainAsync(string subdomain);
        Task<bool> IsSubdomainUniqueAsync(string subdomain, Guid? excludeId = null);
        
        /// <summary>
        /// Busca clínicas públicas ativas com filtros opcionais.
        /// Usado para API pública (sem autenticação).
        /// </summary>
        Task<IEnumerable<Clinic>> SearchPublicClinicsAsync(
            string? name,
            string? city,
            string? state,
            string? clinicType,
            int pageNumber,
            int pageSize);
        
        /// <summary>
        /// Conta total de clínicas públicas ativas com filtros opcionais.
        /// Usado para paginação na API pública.
        /// </summary>
        Task<int> CountPublicClinicsAsync(
            string? name,
            string? city,
            string? state,
            string? clinicType);
        
        /// <summary>
        /// Gets all active clinics.
        /// </summary>
        Task<List<Clinic>> GetAllActiveAsync();
        
        /// <summary>
        /// Gets clinic customization by clinic ID.
        /// </summary>
        Task<ClinicCustomization?> GetCustomizationAsync(Guid clinicId);
    }
}
