using MedicSoft.Application.DTOs.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface ISurveyService
    {
        // Survey CRUD
        Task<SurveyDto> CreateAsync(CreateSurveyDto dto, string tenantId);
        Task<SurveyDto> UpdateAsync(Guid id, UpdateSurveyDto dto, string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
        Task<SurveyDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<SurveyDto>> GetAllAsync(string tenantId);
        Task<IEnumerable<SurveyDto>> GetActiveAsync(string tenantId);
        
        // Activation
        Task<bool> ActivateAsync(Guid id, string tenantId);
        Task<bool> DeactivateAsync(Guid id, string tenantId);
        
        // Response Management
        Task<SurveyResponseDto> SubmitResponseAsync(SubmitSurveyResponseDto dto, string tenantId);
        Task<SurveyResponseDto?> GetResponseAsync(Guid responseId, string tenantId);
        Task<IEnumerable<SurveyResponseDto>> GetPatientResponsesAsync(Guid patientId, string tenantId);
        Task<IEnumerable<SurveyResponseDto>> GetSurveyResponsesAsync(Guid surveyId, string tenantId);
        
        // Analytics
        Task<SurveyAnalyticsDto?> GetAnalyticsAsync(Guid surveyId, string tenantId);
        Task RecalculateMetricsAsync(Guid surveyId, string tenantId);
        
        // Trigger survey for a patient
        Task SendSurveyToPatientAsync(Guid surveyId, Guid patientId, string tenantId);
    }
}
