using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    public interface IImpostoNotaRepository : IRepository<ImpostoNota>
    {
        /// <summary>
        /// Busca os impostos de uma nota fiscal específica
        /// </summary>
        Task<ImpostoNota?> GetByNotaFiscalIdAsync(Guid notaFiscalId, string tenantId);
        
        /// <summary>
        /// Busca todos os impostos de notas de uma clínica em um período
        /// </summary>
        Task<IEnumerable<ImpostoNota>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
        
        /// <summary>
        /// Calcula o total de impostos de uma clínica em um período
        /// </summary>
        Task<decimal> GetTotalImpostosPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
    }
}
