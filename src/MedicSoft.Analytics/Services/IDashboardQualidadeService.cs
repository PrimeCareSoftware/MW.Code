using System;
using System.Threading.Tasks;
using MedicSoft.Analytics.DTOs;

namespace MedicSoft.Analytics.Services
{
    /// <summary>
    /// Interface para serviço de dashboard de qualidade
    /// </summary>
    public interface IDashboardQualidadeService
    {
        /// <summary>
        /// Obtém o dashboard de qualidade para o período especificado
        /// </summary>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Dashboard de qualidade com métricas calculadas</returns>
        Task<DashboardQualidadeDto> GetDashboardAsync(DateTime inicio, DateTime fim, string tenantId);
    }
}
