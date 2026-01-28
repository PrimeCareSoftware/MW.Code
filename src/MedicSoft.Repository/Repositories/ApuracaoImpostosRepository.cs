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
    public class ApuracaoImpostosRepository : BaseRepository<ApuracaoImpostos>, IApuracaoImpostosRepository
    {
        public ApuracaoImpostosRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ApuracaoImpostos?> GetByClinicaAndMesAnoAsync(
            Guid clinicaId, 
            int mes, 
            int ano, 
            string tenantId)
        {
            return await _dbSet
                .Include(a => a.NotasIncluidas)
                .FirstOrDefaultAsync(a => a.ClinicaId == clinicaId 
                                       && a.Mes == mes 
                                       && a.Ano == ano 
                                       && a.TenantId == tenantId);
        }

        public async Task<IEnumerable<ApuracaoImpostos>> GetByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Where(a => a.ClinicaId == clinicaId && a.TenantId == tenantId)
                .OrderByDescending(a => a.Ano)
                .ThenByDescending(a => a.Mes)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApuracaoImpostos>> GetByClinicaAndStatusAsync(
            Guid clinicaId, 
            StatusApuracao status, 
            string tenantId)
        {
            return await _dbSet
                .Where(a => a.ClinicaId == clinicaId 
                         && a.Status == status 
                         && a.TenantId == tenantId)
                .OrderByDescending(a => a.Ano)
                .ThenByDescending(a => a.Mes)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApuracaoImpostos>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            var mesInicio = dataInicio.Month;
            var anoInicio = dataInicio.Year;
            var mesFim = dataFim.Month;
            var anoFim = dataFim.Year;

            return await _dbSet
                .Where(a => a.ClinicaId == clinicaId 
                         && a.TenantId == tenantId
                         && ((a.Ano > anoInicio) || (a.Ano == anoInicio && a.Mes >= mesInicio))
                         && ((a.Ano < anoFim) || (a.Ano == anoFim && a.Mes <= mesFim)))
                .OrderBy(a => a.Ano)
                .ThenBy(a => a.Mes)
                .ToListAsync();
        }
    }
}
