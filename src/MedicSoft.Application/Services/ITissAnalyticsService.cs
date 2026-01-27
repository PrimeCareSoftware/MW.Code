using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for TISS analytics and reporting
    /// </summary>
    public interface ITissAnalyticsService
    {
        /// <summary>
        /// Gets glosas summary for a clinic in a date range
        /// </summary>
        Task<GlosasSummaryDto> GetGlosasSummaryAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets glosas grouped by operator for a clinic
        /// </summary>
        Task<IEnumerable<GlosasByOperatorDto>> GetGlosasByOperatorAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets glosas trend over the last N months
        /// </summary>
        Task<IEnumerable<GlosasTrendDto>> GetGlosasTrendAsync(Guid clinicId, int months, string tenantId);

        /// <summary>
        /// Gets top procedures with glosas
        /// </summary>
        Task<IEnumerable<ProcedureGlosasDto>> GetProcedureGlosasAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets authorization rate by operator
        /// </summary>
        Task<IEnumerable<AuthorizationRateDto>> GetAuthorizationRateAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets average approval time by operator
        /// </summary>
        Task<IEnumerable<ApprovalTimeDto>> GetApprovalTimeAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets monthly performance comparison
        /// </summary>
        Task<IEnumerable<MonthlyPerformanceDto>> GetMonthlyPerformanceAsync(Guid clinicId, int months, string tenantId);

        /// <summary>
        /// Gets glosa alerts (items above average)
        /// </summary>
        Task<IEnumerable<GlosaAlertDto>> GetGlosaAlertsAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        // ====== TISS PHASE 2 - New Analytics Methods ======

        /// <summary>
        /// Gets comprehensive dashboard data with glosas and recursos
        /// </summary>
        Task<DashboardTissDto> GetDashboardDataAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets detailed glosa analytics including recurso tracking
        /// </summary>
        Task<GlosaDetailedAnalyticsDto> GetGlosaDetailedAnalyticsAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets operator performance with glosa and recurso details
        /// </summary>
        Task<IEnumerable<OperadoraPerformanceDto>> GetOperadoraPerformanceAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets glosa trends over time including recurso success rate
        /// </summary>
        Task<IEnumerable<GlosaTendenciaDto>> GetGlosaTendenciasAsync(Guid clinicId, int months, string tenantId);

        /// <summary>
        /// Gets most frequent glosa codes
        /// </summary>
        Task<IEnumerable<GlosaCodigoFrequenteDto>> GetGlosaCodigosFrequentesAsync(Guid clinicId, DateTime startDate, DateTime endDate, int top, string tenantId);

        /// <summary>
        /// Gets procedures most commonly rejected
        /// </summary>
        Task<IEnumerable<ProcedimentoMaisGlosadoDto>> GetProcedimentosMaisGlosadosAsync(Guid clinicId, DateTime startDate, DateTime endDate, int top, string tenantId);
    }
}
