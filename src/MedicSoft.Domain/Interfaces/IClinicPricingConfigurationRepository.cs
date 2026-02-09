using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicPricingConfigurationRepository : IRepository<ClinicPricingConfiguration>
    {
        Task<ClinicPricingConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId);
    }
}
