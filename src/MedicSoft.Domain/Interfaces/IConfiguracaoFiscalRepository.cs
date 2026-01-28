using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    public interface IConfiguracaoFiscalRepository : IRepository<ConfiguracaoFiscal>
    {
        /// <summary>
        /// Busca a configuração fiscal vigente para uma clínica em uma data específica
        /// </summary>
        Task<ConfiguracaoFiscal?> GetConfiguracaoVigenteAsync(Guid clinicaId, DateTime data, string tenantId);
        
        /// <summary>
        /// Busca todas as configurações fiscais de uma clínica
        /// </summary>
        Task<IEnumerable<ConfiguracaoFiscal>> GetByClinicaIdAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Verifica se existe uma configuração fiscal ativa para a clínica
        /// </summary>
        Task<bool> HasConfiguracaoAtivaAsync(Guid clinicaId, string tenantId);
    }
}
