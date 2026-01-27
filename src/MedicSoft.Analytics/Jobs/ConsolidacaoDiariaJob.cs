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

                // Note: Multi-tenant consolidation requires a tenant enumeration service
                // Implementation options:
                // 1. Query distinct TenantIds from Clinic or Company table
                // 2. Use a dedicated TenantService/TenantRepository
                // 3. Queue individual tenant jobs via ExecutarConsolidacaoParaTenantAsync
                
                // For production, implement one of the following approaches:
                /*
                // Option 1: Query all tenants from database
                var tenants = await _context.Clinics
                    .Select(c => c.TenantId)
                    .Distinct()
                    .ToListAsync();
                
                foreach (var tenantId in tenants)
                {
                    await ExecutarConsolidacaoParaTenantAsync(tenantId, dataAnterior);
                }
                */
                
                // Option 2: Use Hangfire to schedule per-tenant jobs
                // This approach provides better isolation and retry handling
                /*
                var tenants = await _tenantService.GetAllActiveTenants();
                foreach (var tenant in tenants)
                {
                    BackgroundJob.Enqueue<ConsolidacaoDiariaJob>(
                        job => job.ExecutarConsolidacaoParaTenantAsync(tenant.Id, dataAnterior));
                }
                */
                
                _logger.LogInformation("Job de consolidação diária executado com sucesso");
                _logger.LogWarning(
                    "Tenant iteration not implemented. Use ExecutarConsolidacaoParaTenantAsync for specific tenants.");
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
