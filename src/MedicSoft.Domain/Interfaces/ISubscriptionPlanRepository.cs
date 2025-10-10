using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
    {
        Task<List<SubscriptionPlan>> GetActiveInPlansAsync();
        new Task<SubscriptionPlan?> GetByIdAsync(Guid id, string tenantId);
        Task<SubscriptionPlan?> GetByTypeAsync(PlanType type, string tenantId);
        Task<IEnumerable<SubscriptionPlan>> GetAllActiveAsync(string tenantId);
    }
}
