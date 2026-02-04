using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class LeadActivityRepository : BaseRepository<LeadActivity>
    {
        public LeadActivityRepository(MedicSoftDbContext context) : base(context)
        {
        }
    }
}
