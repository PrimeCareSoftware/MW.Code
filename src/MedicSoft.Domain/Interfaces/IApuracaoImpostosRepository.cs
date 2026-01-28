using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    public interface IApuracaoImpostosRepository : IRepository<ApuracaoImpostos>
    {
        /// <summary>
        /// Busca apuração de impostos de uma clínica em um mês/ano específico
        /// </summary>
        Task<ApuracaoImpostos?> GetByClinicaAndMesAnoAsync(
            Guid clinicaId, 
            int mes, 
            int ano, 
            string tenantId);
        
        /// <summary>
        /// Busca todas as apurações de uma clínica
        /// </summary>
        Task<IEnumerable<ApuracaoImpostos>> GetByClinicaIdAsync(Guid clinicaId, string tenantId);
        
        /// <summary>
        /// Busca apurações de uma clínica por status
        /// </summary>
        Task<IEnumerable<ApuracaoImpostos>> GetByClinicaAndStatusAsync(
            Guid clinicaId, 
            StatusApuracao status, 
            string tenantId);
        
        /// <summary>
        /// Busca apurações de uma clínica em um intervalo de meses
        /// </summary>
        Task<IEnumerable<ApuracaoImpostos>> GetByClinicaAndPeriodoAsync(
            Guid clinicaId, 
            DateTime dataInicio, 
            DateTime dataFim, 
            string tenantId);
    }
}
