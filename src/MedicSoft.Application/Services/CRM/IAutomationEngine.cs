using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface IAutomationEngine
    {
        Task ExecuteAutomationAsync(MarketingAutomation automation, Guid patientId, string tenantId);
        Task CheckAndTriggerAutomationsAsync(JourneyStageEnum? stage, string? eventName, string tenantId);
    }
}
