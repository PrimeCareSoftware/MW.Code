using System;
using System.Threading.Tasks;

namespace MedicSoft.Analytics.Services
{
    public interface IConsolidacaoDadosService
    {
        Task ConsolidarDadosDiarioAsync(DateTime data, string tenantId);
        Task ConsolidarPeriodoAsync(DateTime dataInicio, DateTime dataFim, string tenantId);
    }
}
