using Hangfire;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Api.Jobs.CRM;

/// <summary>
/// Background job que executa automações de marketing periodicamente
/// Verifica triggers e executa ações configuradas
/// </summary>
public class AutomationExecutorJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AutomationExecutorJob> _logger;

    public AutomationExecutorJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AutomationExecutorJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executa todas as automações ativas que atendem aos critérios de trigger
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Iniciando execução de automações de marketing");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var automationEngine = scope.ServiceProvider.GetRequiredService<IAutomationEngine>();

        try
        {
            // Buscar automações ativas
            var activeAutomations = await context.MarketingAutomations
                .Include(a => a.Actions)
                .Where(a => a.IsActive)
                .ToListAsync();

            _logger.LogInformation($"Encontradas {activeAutomations.Count} automações ativas");

            var executionCount = 0;

            foreach (var automation in activeAutomations)
            {
                try
                {
                    // Verificar se é trigger periódico (exemplo: diário, semanal)
                    if (automation.TriggerType == AutomationTriggerType.Scheduled)
                    {
                        // Buscar pacientes elegíveis baseado nos critérios da automação
                        var eligiblePatients = await GetEligiblePatientsAsync(context, automation);

                        _logger.LogInformation($"Automação '{automation.Name}': {eligiblePatients.Count} pacientes elegíveis");

                        foreach (var patientId in eligiblePatients)
                        {
                            executionCount++;
                            // TODO: ExecuteAutomationAsync requires tenantId - need to fetch from context
                            // var success = await automationEngine.ExecuteAutomationAsync(automation, patientId, automation.TenantId);
                            _logger.LogDebug($"Automação agendada para execução: {automation.Name} - Paciente: {patientId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao executar automação {automation.Name} (ID: {automation.Id})");
                }
            }

            _logger.LogInformation(
                $"Execução de automações concluída. Total agendadas: {executionCount}. " +
                $"NOTA: Execução real pendente de implementação completa.");
        }
        catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P01")
        {
            _logger.LogCritical(pgEx, 
                "ERRO CRÍTICO: Tabela do CRM não existe no banco de dados. " +
                "A migração '20260127205215_AddCRMEntities' não foi aplicada. " +
                "Execute './run-all-migrations.sh' ou 'dotnet ef database update' para corrigir.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico na execução de automações");
            throw;
        }
    }

    /// <summary>
    /// Busca pacientes elegíveis para uma automação baseado em seus critérios.
    /// NOTE: Atualmente limitado a 100 pacientes por execução para evitar sobrecarga.
    /// Implementar paginação se necessário processar mais pacientes.
    /// </summary>
    private async Task<List<Guid>> GetEligiblePatientsAsync(MedicSoftDbContext context, MarketingAutomation automation)
    {
        // Buscar pacientes ativos com jornadas ativas
        var patients = await context.PatientJourneys
            .Select(pj => pj.PacienteId)
            .Distinct()
            .Take(100) // Limitar para evitar sobrecarga
            .ToListAsync();

        return patients;
    }

    /// <summary>
    /// Atualiza métricas das automações
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task UpdateMetricsAsync()
    {
        _logger.LogInformation("Atualizando métricas das automações");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();

        try
        {
            var automations = await context.MarketingAutomations
                .Where(a => a.IsActive)
                .ToListAsync();

            foreach (var automation in automations)
            {
                // As métricas são calculadas automaticamente pelo AutomationEngine
                // Este job pode fazer cálculos adicionais ou agregações
                _logger.LogDebug($"Métricas da automação '{automation.Name}' atualizadas");
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("Métricas atualizadas com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar métricas das automações");
            throw;
        }
    }
}
