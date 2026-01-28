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
    /// Repositório para Balanço Patrimonial
    /// </summary>
    public class BalancoPatrimonialRepository : BaseRepository<BalancoPatrimonial>, IBalancoPatrimonialRepository
    {
        public BalancoPatrimonialRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<BalancoPatrimonial?> GetByDataReferenciaAsync(Guid clinicaId, DateTime dataReferencia, string tenantId)
        {
            return await _dbSet
                .Where(b => b.ClinicaId == clinicaId 
                         && b.TenantId == tenantId
                         && b.DataReferencia == dataReferencia.Date)
                .FirstOrDefaultAsync();
        }
    }
}
