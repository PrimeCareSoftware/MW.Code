using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IUserClinicLinkRepository : IRepository<UserClinicLink>
    {
        Task<IEnumerable<UserClinicLink>> GetByUserIdAsync(Guid userId, string tenantId);
        Task<IEnumerable<UserClinicLink>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<UserClinicLink?> GetByUserAndClinicAsync(Guid userId, Guid clinicId, string tenantId);
        Task<IEnumerable<Clinic>> GetUserClinicsAsync(Guid userId, string tenantId);
        Task<bool> UserHasAccessToClinicAsync(Guid userId, Guid clinicId, string tenantId);
    }
}
