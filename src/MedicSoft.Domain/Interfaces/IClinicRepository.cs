using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicRepository : IRepository<Clinic>
    {
        Task<Clinic?> GetByDocumentAsync(string document, string tenantId);
        Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null);
        Task<Clinic?> GetByCNPJAsync(string cnpj);
    }
}
