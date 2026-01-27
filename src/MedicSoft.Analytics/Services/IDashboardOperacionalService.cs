using System;
using System.Threading.Tasks;
using MedicSoft.Analytics.DTOs;

namespace MedicSoft.Analytics.Services
{
    /// <summary>
    /// Interface para serviço de dashboard operacional
    /// </summary>
    public interface IDashboardOperacionalService
    {
        /// <summary>
        /// Obtém o dashboard operacional para o período especificado
        /// </summary>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Dashboard operacional com métricas calculadas</returns>
        Task<DashboardOperacionalDto> GetDashboardAsync(DateTime inicio, DateTime fim, string tenantId);
    }
}
