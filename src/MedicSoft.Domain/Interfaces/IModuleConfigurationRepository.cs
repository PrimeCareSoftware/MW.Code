using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IModuleConfigurationRepository : IRepository<ModuleConfiguration>
    {
        Task<ModuleConfiguration?> GetByClinicAndModuleAsync(Guid clinicId, string moduleName, string tenantId);
        Task<IEnumerable<ModuleConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId);
    }
}
