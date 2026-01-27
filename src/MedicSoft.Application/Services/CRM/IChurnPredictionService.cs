using MedicSoft.Application.DTOs.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface IChurnPredictionService
    {
        /// <summary>
        /// Prediz o risco de churn de um paciente
        /// </summary>
        Task<ChurnPredictionResultDto> PredictChurnAsync(Guid patientId, string tenantId);
        
        /// <summary>
        /// Obtém pacientes com alto risco de churn
        /// </summary>
        Task<IEnumerable<PatientChurnRiskDto>> GetHighRiskPatientsAsync(string tenantId);
        
        /// <summary>
        /// Obtém os fatores de churn de um paciente específico
        /// </summary>
        Task<IEnumerable<ChurnFactorDto>> GetChurnFactorsAsync(Guid patientId, string tenantId);
        
        /// <summary>
        /// Recalcula todas as predições de churn
        /// </summary>
        Task RecalculateAllPredictionsAsync(string tenantId);
    }
}
