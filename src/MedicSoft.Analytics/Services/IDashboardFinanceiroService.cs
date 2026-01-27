using System;
using System.Threading.Tasks;
using MedicSoft.Analytics.DTOs;

namespace MedicSoft.Analytics.Services
{
    public interface IDashboardFinanceiroService
    {
        Task<DashboardFinanceiroDto> GetDashboardAsync(DateTime inicio, DateTime fim, string tenantId);
        Task<decimal> ProjetarReceitaMesAsync(DateTime mes, string tenantId);
    }
}
