using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
    {
        Task<List<SubscriptionPlan>> GetActiveInPlansAsync();
    }
}
