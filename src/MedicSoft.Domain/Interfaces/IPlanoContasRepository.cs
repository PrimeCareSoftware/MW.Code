using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPlanoContasRepository : IRepository<PlanoContas>
    {
        /// <summary>
        /// Busca todas as contas de uma clínica
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetByClinicaIdAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Busca contas ativas de uma clínica
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetAtivasByClinicaIdAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Busca contas analíticas (que aceitam lançamentos) de uma clínica
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetAnaliticasByClinicaIdAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Busca uma conta pelo código
        /// </summary>
        Task<PlanoContas?> GetByCodigoAsync(Guid clinicaId, string codigo, string tenantId);
        
        /// <summary>
        /// Busca subcontas de uma conta pai
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetSubContasAsync(Guid contaPaiId, string tenantId);
        
        /// <summary>
        /// Busca contas de nível raiz (sem conta pai)
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetContasRaizAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Busca contas por tipo
        /// </summary>
        Task<IEnumerable<PlanoContas>> GetByTipoAsync(
            Guid clinicaId, 
            TipoConta tipo, 
            string tenantId);
    }
}
