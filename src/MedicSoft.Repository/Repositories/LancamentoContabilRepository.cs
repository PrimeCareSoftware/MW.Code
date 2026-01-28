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
    public class LancamentoContabilRepository : BaseRepository<LancamentoContabil>, ILancamentoContabilRepository
    {
        public LancamentoContabilRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LancamentoContabil>> GetByContaIdAsync(
            Guid planoContasId, 
            string tenantId)
        {
            return await _dbSet
                .Include(l => l.Conta)
                .Where(l => l.PlanoContasId == planoContasId && l.TenantId == tenantId)
                .OrderBy(l => l.DataLancamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<LancamentoContabil>> GetByContaAndPeriodoAsync(
            Guid planoContasId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            return await _dbSet
                .Include(l => l.Conta)
                .Where(l => l.PlanoContasId == planoContasId 
                         && l.DataLancamento >= dataInicio 
                         && l.DataLancamento <= dataFim 
                         && l.TenantId == tenantId)
                .OrderBy(l => l.DataLancamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<LancamentoContabil>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            return await _dbSet
                .Include(l => l.Conta)
                .Where(l => l.ClinicaId == clinicaId 
                         && l.DataLancamento >= dataInicio 
                         && l.DataLancamento <= dataFim 
                         && l.TenantId == tenantId)
                .OrderBy(l => l.DataLancamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<LancamentoContabil>> GetByLoteIdAsync(Guid loteId, string tenantId)
        {
            return await _dbSet
                .Include(l => l.Conta)
                .Where(l => l.LoteId == loteId && l.TenantId == tenantId)
                .OrderBy(l => l.DataLancamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<LancamentoContabil>> GetByDocumentoOrigemAsync(
            Guid documentoOrigemId, 
            string tenantId)
        {
            return await _dbSet
                .Include(l => l.Conta)
                .Where(l => l.DocumentoOrigemId == documentoOrigemId && l.TenantId == tenantId)
                .OrderBy(l => l.DataLancamento)
                .ToListAsync();
        }

        public async Task<decimal> GetSaldoContaAsync(
            Guid planoContasId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId)
        {
            var lancamentos = await _dbSet
                .Where(l => l.PlanoContasId == planoContasId 
                         && l.DataLancamento >= dataInicio 
                         && l.DataLancamento <= dataFim 
                         && l.TenantId == tenantId)
                .ToListAsync();

            var totalDebitos = lancamentos
                .Where(l => l.Tipo == TipoLancamentoContabil.Debito)
                .Sum(l => l.Valor);

            var totalCreditos = lancamentos
                .Where(l => l.Tipo == TipoLancamentoContabil.Credito)
                .Sum(l => l.Valor);

            // O saldo depende da natureza da conta, mas aqui retornamos a diferença
            // A lógica de saldo será tratada na camada de serviço
            return totalDebitos - totalCreditos;
        }
    }
}
