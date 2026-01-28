using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Services
{
    public interface ICalculoImpostosService
    {
        /// <summary>
        /// Calcula impostos automaticamente para uma nota fiscal
        /// </summary>
        Task<ImpostoNota> CalcularImpostosAsync(Guid notaFiscalId, string tenantId);
        
        /// <summary>
        /// Recalcula impostos de uma nota existente
        /// </summary>
        Task<ImpostoNota> RecalcularImpostosAsync(Guid notaFiscalId, string tenantId);
    }
}
