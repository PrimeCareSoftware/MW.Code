using Hangfire;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Api.Jobs.CRM;

/// <summary>
/// Background job que realiza predição de churn periodicamente
/// para identificar pacientes em risco
/// </summary>
public class ChurnPredictionJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ChurnPredictionJob> _logger;

    public ChurnPredictionJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ChurnPredictionJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executa predição de churn para todos os pacientes ativos
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task PredictChurnAsync()
    {
        _logger.LogInformation("Iniciando predição de churn em batch");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            // Buscar todos os pacientes ativos (usando PatientJourneys como proxy)
            var activePacientes = await context.PatientJourneys
                .Select(pj => pj.PacienteId)
                .Distinct()
                .ToListAsync();

            _logger.LogInformation($"Processando predição de churn para {activePacientes.Count} pacientes");

            var processedCount = 0;
            var highRiskCount = 0;
            var criticalRiskCount = 0;

            // TODO: Implementar processamento em lotes
            // Requer integração com ChurnPredictionService

            _logger.LogInformation(
                $"Predição de churn concluída. Total: {processedCount}, " +
                $"Alto Risco: {highRiskCount}, Risco Crítico: {criticalRiskCount}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico na predição de churn");
            throw;
        }
    }

    /// <summary>
    /// Identifica e notifica sobre pacientes de alto risco
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task NotifyHighRiskPatientsAsync()
    {
        _logger.LogInformation("Identificando pacientes de alto risco de churn");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            var highRiskPatients = await context.ChurnPredictions
                .Where(cp => cp.RiskLevel == ChurnRiskLevel.High || cp.RiskLevel == ChurnRiskLevel.Critical)
                .ToListAsync();

            _logger.LogInformation($"Encontrados {highRiskPatients.Count} pacientes de alto risco");

            // TODO: Implementar notificações
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao notificar pacientes de alto risco");
            throw;
        }
    }

    /// <summary>
    /// Recalcula predições antigas
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task RecalculateOldPredictionsAsync()
    {
        _logger.LogInformation("Recalculando predições antigas");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            // Buscar predições com mais de 7 dias
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var oldPredictions = await context.ChurnPredictions
                .Where(cp => cp.PredictedAt < sevenDaysAgo)
                .Select(cp => cp.PatientId)
                .Distinct()
                .ToListAsync();

            _logger.LogInformation($"Recalculando {oldPredictions.Count} predições antigas");

            // TODO: Implementar recálculo

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recalcular predições antigas");
            throw;
        }
    }

    /// <summary>
    /// Analisa efetividade das ações de retenção
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task AnalyzeRetentionEffectivenessAsync()
    {
        _logger.LogInformation("Analisando efetividade das ações de retenção");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            // TODO: Implementar análise de efetividade
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var oldHighRiskPredictions = await context.ChurnPredictions
                .Where(cp => cp.PredictedAt < thirtyDaysAgo)
                .Where(cp => cp.RiskLevel == ChurnRiskLevel.High || cp.RiskLevel == ChurnRiskLevel.Critical)
                .ToListAsync();

            _logger.LogInformation($"Análise de retenção (últimos 30 dias): Total analisado: {oldHighRiskPredictions.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao analisar efetividade de retenção");
            throw;
        }
    }
}
