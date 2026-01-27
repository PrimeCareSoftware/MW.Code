using Hangfire;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Api.Jobs.CRM;

/// <summary>
/// Background job que realiza análise de sentimento em batch
/// para comentários, reclamações e respostas de pesquisas
/// </summary>
public class SentimentAnalysisJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SentimentAnalysisJob> _logger;

    public SentimentAnalysisJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SentimentAnalysisJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Analisa comentários não processados de surveys
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task AnalyzeSurveyCommentsAsync()
    {
        _logger.LogInformation("Iniciando análise de sentimento de comentários de pesquisas");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var sentimentService = scope.ServiceProvider.GetRequiredService<ISentimentAnalysisService>();

        try
        {
            // Buscar respostas de pesquisas com comentários não analisados
            var unanalyzedResponses = await context.SurveyQuestionResponses
                .Where(sqr => sqr.TextAnswer != null && sqr.TextAnswer.Length > 10)
                .Where(sqr => !context.SentimentAnalyses.Any(sa => 
                    sa.SourceType == "SurveyResponse" && 
                    sa.SourceId == sqr.Id))
                .Include(sqr => sqr.SurveyResponse)
                .Take(100) // Processar em lotes
                .ToListAsync();

            _logger.LogInformation($"Encontradas {unanalyzedResponses.Count} respostas não analisadas");

            var analyzedCount = 0;
            var positiveCount = 0;
            var negativeCount = 0;
            var neutralCount = 0;

            foreach (var response in unanalyzedResponses)
            {
                try
                {
                    // Note: The sentiment analysis is saved automatically by the service
                    // but without association to the specific survey response
                    var result = await sentimentService.AnalyzeTextAsync(
                        response.TextAnswer,
                        response.SurveyResponse.TenantId);

                    analyzedCount++;

                    switch (result.Sentiment)
                    {
                        case Domain.Entities.CRM.SentimentType.Positive:
                            positiveCount++;
                            break;
                        case Domain.Entities.CRM.SentimentType.Negative:
                            negativeCount++;
                            _logger.LogWarning($"Sentimento negativo detectado em resposta de survey: {response.TextAnswer.Substring(0, Math.Min(50, response.TextAnswer.Length))}...");
                            break;
                        case Domain.Entities.CRM.SentimentType.Neutral:
                            neutralCount++;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao analisar resposta de survey {response.Id}");
                }
            }

            _logger.LogInformation(
                $"Análise de comentários de surveys concluída. " +
                $"Total: {analyzedCount}, Positivos: {positiveCount}, " +
                $"Neutros: {neutralCount}, Negativos: {negativeCount}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na análise de comentários de surveys");
            throw;
        }
    }

    /// <summary>
    /// Analisa descrições e interações de reclamações
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task AnalyzeComplaintsAsync()
    {
        _logger.LogInformation("Iniciando análise de sentimento de reclamações");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var sentimentService = scope.ServiceProvider.GetRequiredService<ISentimentAnalysisService>();

        try
        {
            // Buscar reclamações não analisadas
            var unanalyzedComplaints = await context.Complaints
                .Where(c => !context.SentimentAnalyses.Any(sa => 
                    sa.SourceType == "Complaint" && 
                    sa.SourceId == c.Id))
                .Take(50) // Processar em lotes menores para reclamações
                .ToListAsync();

            _logger.LogInformation($"Encontradas {unanalyzedComplaints.Count} reclamações não analisadas");

            var analyzedCount = 0;

            foreach (var complaint in unanalyzedComplaints)
            {
                try
                {
                    var result = await sentimentService.AnalyzeTextAsync(
                        complaint.Description,
                        complaint.TenantId);

                    analyzedCount++;

                    if (result.Sentiment == Domain.Entities.CRM.SentimentType.Negative)
                    {
                        _logger.LogWarning($"Reclamação com sentimento negativo: {complaint.ProtocolNumber}");
                        
                        // Poderia disparar ações automáticas aqui:
                        // - Aumentar prioridade da reclamação
                        // - Notificar supervisores
                        // - Criar task urgente
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao analisar reclamação {complaint.Id}");
                }
            }

            _logger.LogInformation($"Análise de reclamações concluída. Total analisado: {analyzedCount}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na análise de reclamações");
            throw;
        }
    }

    /// <summary>
    /// Analisa interações de reclamações (respostas, atualizações)
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task AnalyzeComplaintInteractionsAsync()
    {
        _logger.LogInformation("Iniciando análise de sentimento de interações de reclamações");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var sentimentService = scope.ServiceProvider.GetRequiredService<ISentimentAnalysisService>();

        try
        {
            // Buscar interações não analisadas
            var unanalyzedInteractions = await context.ComplaintInteractions
                .Where(ci => !context.SentimentAnalyses.Any(sa => 
                    sa.SourceType == "ComplaintInteraction" && 
                    sa.SourceId == ci.Id))
                .Include(ci => ci.Complaint)
                .Take(100)
                .ToListAsync();

            _logger.LogInformation($"Encontradas {unanalyzedInteractions.Count} interações não analisadas");

            var analyzedCount = 0;

            foreach (var interaction in unanalyzedInteractions)
            {
                try
                {
                    var result = await sentimentService.AnalyzeTextAsync(
                        interaction.Message,
                        interaction.TenantId);

                    analyzedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao analisar interação {interaction.Id}");
                }
            }

            _logger.LogInformation($"Análise de interações concluída. Total analisado: {analyzedCount}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na análise de interações");
            throw;
        }
    }

    /// <summary>
    /// Gera alertas para sentimentos negativos críticos
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task GenerateNegativeSentimentAlertsAsync()
    {
        _logger.LogInformation("Gerando alertas de sentimento negativo");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            // Buscar análises negativas recentes (últimas 24 horas) não alertadas
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);

            var recentNegativeAnalyses = await context.SentimentAnalyses
                .Where(sa => sa.AnalyzedAt >= oneDayAgo)
                .Where(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Negative)
                .ToListAsync();

            _logger.LogInformation($"Encontradas {recentNegativeAnalyses.Count} análises negativas recentes");

            if (!recentNegativeAnalyses.Any())
            {
                return;
            }

            foreach (var analysis in recentNegativeAnalyses)
            {
                try
                {
                    // Gerar alerta
                    _logger.LogWarning(
                        $"ALERTA SENTIMENTO NEGATIVO: " +
                        $"Fonte: {analysis.SourceType}, " +
                        $"Confidence: {analysis.ConfidenceScore:P0}, " +
                        $"Texto: {analysis.SourceText.Substring(0, Math.Min(100, analysis.SourceText.Length))}...");
                    
                    // Aqui você poderia:
                    // 1. Enviar notificação por email para supervisores
                    // 2. Criar task urgente no sistema
                    // 3. Disparar automação de retenção
                    // 4. Registrar no dashboard de alertas
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao gerar alerta para análise {analysis.Id}");
                }
            }

            _logger.LogInformation($"Alertas de sentimento negativo gerados: {recentNegativeAnalyses.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar alertas de sentimento negativo");
            throw;
        }
    }

    /// <summary>
    /// Analisa tendências de sentimento
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task AnalyzeSentimentTrendsAsync()
    {
        _logger.LogInformation("Analisando tendências de sentimento");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var recentAnalyses = await context.SentimentAnalyses
                .Where(sa => sa.AnalyzedAt >= thirtyDaysAgo)
                .ToListAsync();

            if (!recentAnalyses.Any())
            {
                _logger.LogInformation("Sem análises recentes para calcular tendências");
                return;
            }

            var totalCount = recentAnalyses.Count;
            var positiveCount = recentAnalyses.Count(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Positive);
            var negativeCount = recentAnalyses.Count(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Negative);
            var neutralCount = recentAnalyses.Count(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Neutral);

            var positivePercentage = (double)positiveCount / totalCount * 100;
            var negativePercentage = (double)negativeCount / totalCount * 100;
            var neutralPercentage = (double)neutralCount / totalCount * 100;

            _logger.LogInformation(
                $"Tendências de sentimento (últimos 30 dias): " +
                $"Total: {totalCount}, " +
                $"Positivo: {positivePercentage:F1}%, " +
                $"Neutro: {neutralPercentage:F1}%, " +
                $"Negativo: {negativePercentage:F1}%");

            // Analisar por fonte
            var bySource = recentAnalyses
                .GroupBy(sa => sa.SourceType)
                .Select(g => new
                {
                    Source = g.Key,
                    Total = g.Count(),
                    Positive = g.Count(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Positive),
                    Negative = g.Count(sa => sa.Sentiment == Domain.Entities.CRM.SentimentType.Negative)
                })
                .ToList();

            foreach (var source in bySource)
            {
                var negativeRate = (double)source.Negative / source.Total * 100;
                _logger.LogInformation(
                    $"  {source.Source}: Total={source.Total}, " +
                    $"Positivo={source.Positive}, Negativo={source.Negative} ({negativeRate:F1}%)");

                if (negativeRate > 30) // Mais de 30% negativo
                {
                    _logger.LogWarning($"⚠️  Alta taxa de sentimento negativo em {source.Source}!");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao analisar tendências de sentimento");
            throw;
        }
    }
}
