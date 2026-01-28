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
    public class ImpostoNotaRepository : BaseRepository<ImpostoNota>, IImpostoNotaRepository
    {
        public ImpostoNotaRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ImpostoNota?> GetByNotaFiscalIdAsync(Guid notaFiscalId, string tenantId)
        {
            return await _dbSet
                .Include(i => i.NotaFiscal)
                .FirstOrDefaultAsync(i => i.NotaFiscalId == notaFiscalId && i.TenantId == tenantId);
        }

        public async Task<IEnumerable<ImpostoNota>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            // Get clinic to get CNPJ
            var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == clinicaId);
            if (clinic == null)
                return new List<ImpostoNota>();

            return await _dbSet
                .Include(i => i.NotaFiscal)
                .Where(i => i.TenantId == tenantId
                         && i.NotaFiscal != null
                         && i.NotaFiscal.ProviderCnpj == clinic.Document
                         && i.DataCalculo >= dataInicio
                         && i.DataCalculo <= dataFim)
                .OrderBy(i => i.DataCalculo)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalImpostosPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            // Get clinic to get CNPJ
            var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == clinicaId);
            if (clinic == null)
                return 0;

            var impostos = await _dbSet
                .Include(i => i.NotaFiscal)
                .Where(i => i.TenantId == tenantId
                         && i.NotaFiscal != null
                         && i.NotaFiscal.ProviderCnpj == clinic.Document
                         && i.DataCalculo >= dataInicio
                         && i.DataCalculo <= dataFim)
                .ToListAsync();

            return impostos.Sum(i => i.TotalImpostos);
        }
    }
}
