using MedicSoft.Application.DTOs.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface ISentimentAnalysisService
    {
        /// <summary>
        /// Analisa o sentimento de um texto
        /// </summary>
        Task<SentimentAnalysisResultDto> AnalyzeTextAsync(string text, string tenantId);
        
        /// <summary>
        /// Analisa múltiplos textos em lote
        /// </summary>
        Task<IEnumerable<SentimentAnalysisResultDto>> AnalyzeBatchAsync(List<string> texts, string tenantId);
        
        /// <summary>
        /// Obtém análises de sentimento por entidade
        /// </summary>
        Task<IEnumerable<SentimentAnalysisDto>> GetByEntityAsync(Guid entityId, string entityType, string tenantId);
        
        /// <summary>
        /// Obtém alertas de sentimento negativo
        /// </summary>
        Task<IEnumerable<SentimentAnalysisDto>> GetNegativeAlertsAsync(string tenantId);
    }
}
