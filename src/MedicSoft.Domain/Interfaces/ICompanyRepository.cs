using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByDocumentAsync(string document);
        Task<Company?> GetBySubdomainAsync(string subdomain);
        Task<bool> IsSubdomainUniqueAsync(string subdomain, Guid? excludeId = null);
        Task<bool> IsDocumentUniqueAsync(string document, Guid? excludeId = null);
        Task<IEnumerable<Clinic>> GetCompayClinicsAsync(Guid companyId);
    }
}
