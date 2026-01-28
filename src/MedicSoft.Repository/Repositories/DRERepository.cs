using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repositório para DRE (Demonstração do Resultado do Exercício)
    /// </summary>
    public class DRERepository : BaseRepository<DRE>, IDRERepository
    {
        public DRERepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<DRE?> GetByPeriodoAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            return await _dbSet
                .Where(d => d.ClinicaId == clinicaId 
                         && d.TenantId == tenantId
                         && d.PeriodoInicio == dataInicio.Date
                         && d.PeriodoFim == dataFim.Date)
                .FirstOrDefaultAsync();
        }
    }
}
