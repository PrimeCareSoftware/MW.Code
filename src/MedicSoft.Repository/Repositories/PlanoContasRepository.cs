using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PlanoContasRepository : BaseRepository<PlanoContas>, IPlanoContasRepository
    {
        public PlanoContasRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PlanoContas>> GetByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ClinicaId == clinicaId && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanoContas>> GetAtivasByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ClinicaId == clinicaId && p.Ativa && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanoContas>> GetAnaliticasByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ClinicaId == clinicaId && p.Analitica && p.Ativa && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }

        public async Task<PlanoContas?> GetByCodigoAsync(Guid clinicaId, string codigo, string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.ClinicaId == clinicaId 
                                       && p.Codigo == codigo 
                                       && p.TenantId == tenantId);
        }

        public async Task<IEnumerable<PlanoContas>> GetSubContasAsync(Guid contaPaiId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ContaPaiId == contaPaiId && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanoContas>> GetContasRaizAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.ClinicaId == clinicaId && p.ContaPaiId == null && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanoContas>> GetByTipoAsync(
            Guid clinicaId, 
            TipoConta tipo, 
            string tenantId)
        {
            return await _dbSet
                .Where(p => p.ClinicaId == clinicaId && p.Tipo == tipo && p.TenantId == tenantId)
                .OrderBy(p => p.Codigo)
                .ToListAsync();
        }
    }
}
