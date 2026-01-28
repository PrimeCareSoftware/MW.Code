using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    public interface ILancamentoContabilRepository : IRepository<LancamentoContabil>
    {
        /// <summary>
        /// Busca lançamentos de uma conta específica
        /// </summary>
        Task<IEnumerable<LancamentoContabil>> GetByContaIdAsync(
            Guid planoContasId, 
            string tenantId);
        
        /// <summary>
        /// Busca lançamentos de uma conta em um período
        /// </summary>
        Task<IEnumerable<LancamentoContabil>> GetByContaAndPeriodoAsync(
            Guid planoContasId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
        
        /// <summary>
        /// Busca lançamentos de uma clínica em um período
        /// </summary>
        Task<IEnumerable<LancamentoContabil>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
        
        /// <summary>
        /// Busca lançamentos por lote
        /// </summary>
        Task<IEnumerable<LancamentoContabil>> GetByLoteIdAsync(Guid loteId, string tenantId);
        
        /// <summary>
        /// Busca lançamentos por documento origem
        /// </summary>
        Task<IEnumerable<LancamentoContabil>> GetByDocumentoOrigemAsync(
            Guid documentoOrigemId, 
            string tenantId);
        
        /// <summary>
        /// Calcula o saldo de uma conta em um período
        /// </summary>
        Task<decimal> GetSaldoContaAsync(
            Guid planoContasId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
    }
}
