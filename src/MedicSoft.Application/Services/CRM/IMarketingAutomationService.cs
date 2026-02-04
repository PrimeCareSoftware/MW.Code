using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.DTOs.Common;

namespace MedicSoft.Application.Services.CRM
{
    public interface IMarketingAutomationService
    {
        // CRUD Operations
        Task<MarketingAutomationDto> CreateAsync(CreateMarketingAutomationDto dto, string tenantId);
        Task<MarketingAutomationDto> UpdateAsync(Guid id, UpdateMarketingAutomationDto dto, string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
        Task<MarketingAutomationDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<MarketingAutomationDto>> GetAllAsync(string tenantId);
        Task<PagedResult<MarketingAutomationDto>> GetAllPagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25);
        Task<IEnumerable<MarketingAutomationDto>> GetActiveAsync(string tenantId);
        Task<PagedResult<MarketingAutomationDto>> GetActivePagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25);
        
        // Activation
        Task<bool> ActivateAsync(Guid id, string tenantId);
        Task<bool> DeactivateAsync(Guid id, string tenantId);
        
        // Metrics
        Task<MarketingAutomationMetricsDto?> GetMetricsAsync(Guid id, string tenantId);
        Task<IEnumerable<MarketingAutomationMetricsDto>> GetAllMetricsAsync(string tenantId);
        
        // Execution
        Task TriggerAutomationAsync(Guid automationId, Guid patientId, string tenantId);
    }
}
