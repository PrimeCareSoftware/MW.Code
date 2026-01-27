using System;
using System.Threading.Tasks;
using MedicSoft.Analytics.DTOs;

namespace MedicSoft.Analytics.Services
{
    public interface IDashboardClinicoService
    {
        Task<DashboardClinicoDto> GetDashboardAsync(DateTime inicio, DateTime fim, string tenantId, Guid? medicoId = null);
    }
}
