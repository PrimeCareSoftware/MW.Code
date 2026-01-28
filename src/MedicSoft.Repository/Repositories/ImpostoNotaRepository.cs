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

            // Use Join to filter at database level before loading entities
            return await (from imposto in _dbSet
                          join nota in _context.ElectronicInvoices on imposto.NotaFiscalId equals nota.Id
                          where imposto.TenantId == tenantId
                             && nota.ProviderCnpj == clinic.Document
                             && imposto.DataCalculo >= dataInicio
                             && imposto.DataCalculo <= dataFim
                          orderby imposto.DataCalculo
                          select imposto)
                          .Include(i => i.NotaFiscal)
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

            // Use Join and database-side aggregation
            var result = await (from imposto in _dbSet
                               join nota in _context.ElectronicInvoices on imposto.NotaFiscalId equals nota.Id
                               where imposto.TenantId == tenantId
                                  && nota.ProviderCnpj == clinic.Document
                                  && imposto.DataCalculo >= dataInicio
                                  && imposto.DataCalculo <= dataFim
                               select new
                               {
                                   imposto.ValorPIS,
                                   imposto.ValorCOFINS,
                                   imposto.ValorIR,
                                   imposto.ValorCSLL,
                                   imposto.ValorISS,
                                   imposto.ValorINSS
                               })
                               .ToListAsync();

            return result.Sum(i => i.ValorPIS + i.ValorCOFINS + i.ValorIR + i.ValorCSLL + i.ValorISS + i.ValorINSS);
        }
    }
}
