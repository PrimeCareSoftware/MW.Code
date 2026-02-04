using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class LeadRepository : BaseRepository<Lead>
    {
        public LeadRepository(MedicSoftDbContext context) : base(context)
        {
        }
    }
}
