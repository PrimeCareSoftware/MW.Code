using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicCustomizationRepository : IRepository<ClinicCustomization>
    {
        Task<ClinicCustomization?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<ClinicCustomization?> GetBySubdomainAsync(string subdomain);
    }
}
