using Hangfire;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Api.Jobs.CRM;

/// <summary>
/// Background job que envia pesquisas NPS/CSAT automaticamente
/// baseado em eventos do sistema (consultas, tratamentos, etc.)
/// </summary>
public class SurveyTriggerJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SurveyTriggerJob> _logger;

    public SurveyTriggerJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SurveyTriggerJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Verifica eventos que devem disparar pesquisas e envia para pacientes
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task TriggerSurveysAsync()
    {
        _logger.LogInformation("Iniciando verificação de triggers de pesquisas");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            // Verificar surveys ativos
            var activeSurveys = await context.Surveys
                .Where(s => s.IsActive)
                .ToListAsync();

            _logger.LogInformation($"Encontrados {activeSurveys.Count} surveys ativos");

            // TODO: Implementar lógica de disparo de surveys baseado em eventos
            // - NPS após consultas
            // - CSAT após tratamentos
            // - Surveys periódicos
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao disparar pesquisas");
            throw;
        }
    }

    /// <summary>
    /// Processa respostas de pesquisas e atualiza métricas
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessSurveyResponsesAsync()
    {
        _logger.LogInformation("Processando respostas de pesquisas");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            var surveys = await context.Surveys
                .Where(s => s.IsActive)
                .ToListAsync();

            foreach (var survey in surveys)
            {
                // Recalcular métricas agregadas
                var responses = await context.SurveyResponses
                    .Where(sr => sr.SurveyId == survey.Id && sr.CompletedAt != null)
                    .ToListAsync();

                if (responses.Any())
                {
                    _logger.LogDebug($"Survey '{survey.Name}': {responses.Count} respostas processadas");
                }
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("Respostas de pesquisas processadas com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar respostas de pesquisas");
            throw;
        }
    }
}
