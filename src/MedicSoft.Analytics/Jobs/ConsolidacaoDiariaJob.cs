using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Analytics.Services;

namespace MedicSoft.Analytics.Jobs
{
    /// <summary>
    /// Hangfire job for daily data consolidation
    /// Runs nightly to aggregate data from the previous day
    /// </summary>
    public class ConsolidacaoDiariaJob
    {
        private readonly IConsolidacaoDadosService _consolidacaoService;
        private readonly ILogger<ConsolidacaoDiariaJob> _logger;

        public ConsolidacaoDiariaJob(
            IConsolidacaoDadosService consolidacaoService,
            ILogger<ConsolidacaoDiariaJob> logger)
        {
            _consolidacaoService = consolidacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Executes daily consolidation for all tenants
        /// Called by Hangfire scheduler daily at 00:00
        /// </summary>
        public async Task ExecutarConsolidacaoDiariaAsync()
        {
            try
            {
                var dataAnterior = DateTime.UtcNow.Date.AddDays(-1);
                _logger.LogInformation("Iniciando consolidação diária para data: {Data}", dataAnterior);

                // Note: In a multi-tenant scenario, you would need to iterate through all tenants
                // For now, the consolidation service should handle this internally or be called per tenant
                // This is a placeholder implementation
                
                // TODO: Get all active tenants and consolidate for each
                // For now, consolidation will be triggered per tenant via the API or tenant-aware service
                
                _logger.LogInformation("Job de consolidação diária executado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar job de consolidação diária");
                throw; // Rethrow to mark job as failed in Hangfire
            }
        }

        /// <summary>
        /// Executes consolidation for a specific date and tenant
        /// Can be called manually or scheduled per tenant
        /// </summary>
        public async Task ExecutarConsolidacaoParaTenantAsync(string tenantId, DateTime data)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando consolidação para tenant {TenantId} na data {Data}", 
                    tenantId, 
                    data);

                await _consolidacaoService.ConsolidarDadosDiarioAsync(data, tenantId);

                _logger.LogInformation(
                    "Consolidação concluída com sucesso para tenant {TenantId} na data {Data}", 
                    tenantId, 
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Erro ao consolidar dados para tenant {TenantId} na data {Data}", 
                    tenantId, 
                    data);
                throw;
            }
        }
    }
}
