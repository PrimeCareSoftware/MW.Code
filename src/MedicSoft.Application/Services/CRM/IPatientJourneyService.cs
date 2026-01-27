using MedicSoft.Application.DTOs.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface IPatientJourneyService
    {
        // Journey Management
        Task<PatientJourneyDto> GetOrCreateJourneyAsync(Guid pacienteId, string tenantId);
        Task<PatientJourneyDto?> GetJourneyByIdAsync(Guid journeyId, string tenantId);
        Task<PatientJourneyDto?> GetJourneyByPatientIdAsync(Guid pacienteId, string tenantId);
        
        // Stage Management
        Task<PatientJourneyDto> AdvanceStageAsync(Guid pacienteId, AdvanceJourneyStageDto dto, string tenantId);
        
        // Touchpoint Management
        Task<PatientJourneyDto> AddTouchpointAsync(Guid pacienteId, CreatePatientTouchpointDto dto, string tenantId);
        
        // Metrics Management
        Task<PatientJourneyDto> UpdateMetricsAsync(Guid pacienteId, UpdatePatientJourneyMetricsDto dto, string tenantId);
        Task<PatientJourneyMetricsDto?> GetMetricsAsync(Guid pacienteId, string tenantId);
        Task RecalculateMetricsAsync(Guid pacienteId, string tenantId);
    }
}
