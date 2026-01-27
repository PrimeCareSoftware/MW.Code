using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<SentimentAnalysisService> _logger;

        // Keywords para detecção simples de sentimento
        private static readonly string[] PositiveKeywords = { "excelente", "ótimo", "bom", "satisfeito", "feliz", "maravilhoso", "perfeito", "adorei", "recomendo" };
        private static readonly string[] NegativeKeywords = { "ruim", "péssimo", "insatisfeito", "problema", "reclamação", "horrível", "terrível", "nunca mais", "decepcionado" };

        public SentimentAnalysisService(
            MedicSoftDbContext context,
            ILogger<SentimentAnalysisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SentimentAnalysisResultDto> AnalyzeTextAsync(string text, string tenantId)
        {
            _logger.LogInformation("Analyzing sentiment for text in tenant {TenantId}", tenantId);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be empty", nameof(text));
            }

            // Análise simples baseada em keywords
            var result = AnalyzeTextKeywordBased(text);

            // Salvar no banco
            var analysis = new SentimentAnalysis(text, "Manual", null, tenantId);
            analysis.SetAnalysisResult(
                result.Sentiment,
                result.PositiveScore,
                result.NeutralScore,
                result.NegativeScore,
                result.ConfidenceScore);

            foreach (var topic in result.Topics)
            {
                analysis.AddTopic(topic);
            }

            _context.SentimentAnalyses.Add(analysis);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Sentiment analysis completed: {Sentiment} (confidence: {Confidence})", 
                result.Sentiment, result.ConfidenceScore);

            return result;
        }

        public async Task<IEnumerable<SentimentAnalysisResultDto>> AnalyzeBatchAsync(List<string> texts, string tenantId)
        {
            _logger.LogInformation("Analyzing batch of {Count} texts in tenant {TenantId}", texts.Count, tenantId);

            var results = new List<SentimentAnalysisResultDto>();

            foreach (var text in texts)
            {
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                var result = await AnalyzeTextAsync(text, tenantId);
                results.Add(result);
            }

            return results;
        }

        public async Task<IEnumerable<SentimentAnalysisDto>> GetByEntityAsync(Guid entityId, string entityType, string tenantId)
        {
            _logger.LogInformation("Getting sentiment analyses for entity {EntityId} of type {EntityType}", 
                entityId, entityType);

            var analyses = await _context.SentimentAnalyses
                .Where(s => s.TenantId == tenantId 
                    && s.SourceId == entityId 
                    && s.SourceType == entityType)
                .OrderByDescending(s => s.AnalyzedAt)
                .ToListAsync();

            return analyses.Select(MapToDto);
        }

        public async Task<IEnumerable<SentimentAnalysisDto>> GetNegativeAlertsAsync(string tenantId)
        {
            _logger.LogInformation("Getting negative sentiment alerts for tenant {TenantId}", tenantId);

            var analyses = await _context.SentimentAnalyses
                .Where(s => s.TenantId == tenantId 
                    && s.Sentiment == SentimentType.Negative
                    && s.CreatedAt >= DateTime.UtcNow.AddDays(-30)) // Últimos 30 dias
                .OrderByDescending(s => s.NegativeScore)
                .Take(50)
                .ToListAsync();

            return analyses.Select(MapToDto);
        }

        // Análise simples baseada em keywords
        private SentimentAnalysisResultDto AnalyzeTextKeywordBased(string text)
        {
            var lowerText = text.ToLowerInvariant();
            
            int positiveCount = PositiveKeywords.Count(k => lowerText.Contains(k));
            int negativeCount = NegativeKeywords.Count(k => lowerText.Contains(k));
            
            double positiveScore = 0;
            double negativeScore = 0;
            double neutralScore = 0;
            SentimentType sentiment;

            if (positiveCount > negativeCount)
            {
                sentiment = SentimentType.Positive;
                positiveScore = Math.Min(1.0, 0.5 + (positiveCount * 0.15));
                negativeScore = Math.Min(0.3, negativeCount * 0.1);
                neutralScore = 1.0 - positiveScore - negativeScore;
            }
            else if (negativeCount > positiveCount)
            {
                sentiment = SentimentType.Negative;
                negativeScore = Math.Min(1.0, 0.5 + (negativeCount * 0.15));
                positiveScore = Math.Min(0.3, positiveCount * 0.1);
                neutralScore = 1.0 - positiveScore - negativeScore;
            }
            else if (positiveCount > 0 && negativeCount > 0)
            {
                sentiment = SentimentType.Mixed;
                positiveScore = positiveCount * 0.2;
                negativeScore = negativeCount * 0.2;
                neutralScore = 1.0 - positiveScore - negativeScore;
            }
            else
            {
                sentiment = SentimentType.Neutral;
                neutralScore = 0.8;
                positiveScore = 0.1;
                negativeScore = 0.1;
            }

            var confidence = Math.Abs(positiveScore - negativeScore);

            return new SentimentAnalysisResultDto
            {
                Sentiment = sentiment,
                SentimentName = sentiment.ToString(),
                PositiveScore = Math.Round(positiveScore, 2),
                NeutralScore = Math.Round(neutralScore, 2),
                NegativeScore = Math.Round(negativeScore, 2),
                ConfidenceScore = Math.Round(confidence, 2),
                Topics = ExtractTopics(text),
                IsNegativeAlert = sentiment == SentimentType.Negative && negativeScore > 0.6
            };
        }

        private List<string> ExtractTopics(string text)
        {
            var topics = new List<string>();
            var lowerText = text.ToLowerInvariant();

            // Tópicos comuns em saúde
            if (lowerText.Contains("atendimento")) topics.Add("Atendimento");
            if (lowerText.Contains("médico") || lowerText.Contains("doutor")) topics.Add("Profissional");
            if (lowerText.Contains("espera") || lowerText.Contains("demora")) topics.Add("Tempo de Espera");
            if (lowerText.Contains("consulta")) topics.Add("Consulta");
            if (lowerText.Contains("exame")) topics.Add("Exame");
            if (lowerText.Contains("preço") || lowerText.Contains("valor") || lowerText.Contains("custo")) topics.Add("Preço");
            if (lowerText.Contains("marcação") || lowerText.Contains("agendamento")) topics.Add("Agendamento");
            if (lowerText.Contains("recepção")) topics.Add("Recepção");

            return topics;
        }

        private SentimentAnalysisDto MapToDto(SentimentAnalysis entity)
        {
            return new SentimentAnalysisDto
            {
                Id = entity.Id,
                SourceText = entity.SourceText,
                SourceType = entity.SourceType,
                SourceId = entity.SourceId,
                Sentiment = entity.Sentiment,
                SentimentName = entity.Sentiment.ToString(),
                PositiveScore = entity.PositiveScore,
                NeutralScore = entity.NeutralScore,
                NegativeScore = entity.NegativeScore,
                ConfidenceScore = entity.ConfidenceScore,
                Topics = entity.Topics.ToList(),
                AnalyzedAt = entity.AnalyzedAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
