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
    public class ConfiguracaoFiscalRepository : BaseRepository<ConfiguracaoFiscal>, IConfiguracaoFiscalRepository
    {
        public ConfiguracaoFiscalRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ConfiguracaoFiscal?> GetConfiguracaoVigenteAsync(Guid clinicaId, DateTime data, string tenantId)
        {
            return await _dbSet
                .Where(c => c.ClinicaId == clinicaId 
                         && c.TenantId == tenantId
                         && c.VigenciaInicio <= data
                         && (c.VigenciaFim == null || c.VigenciaFim >= data))
                .OrderByDescending(c => c.VigenciaInicio)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ConfiguracaoFiscal>> GetByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(c => c.ClinicaId == clinicaId && c.TenantId == tenantId)
                .OrderByDescending(c => c.VigenciaInicio)
                .ToListAsync();
        }

        public async Task<bool> HasConfiguracaoAtivaAsync(Guid clinicaId, string tenantId)
        {
            var hoje = DateTime.UtcNow;
            return await _dbSet
                .AnyAsync(c => c.ClinicaId == clinicaId 
                            && c.TenantId == tenantId
                            && c.VigenciaInicio <= hoje
                            && (c.VigenciaFim == null || c.VigenciaFim >= hoje));
        }
    }
}
