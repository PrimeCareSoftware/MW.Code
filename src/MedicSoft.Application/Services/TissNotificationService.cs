using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for TISS notification system
    /// Handles notifications for glosas, recursos, and alerts
    /// </summary>
    public class TissNotificationService : ITissNotificationService
    {
        private readonly ITissGlosaRepository _glosaRepository;
        private readonly ITissGuideRepository _guideRepository;
        private readonly IHealthInsuranceOperatorRepository _operatorRepository;
        private readonly ILogger<TissNotificationService> _logger;
        // Note: Email service would be injected here in production
        // private readonly IEmailService _emailService;

        public TissNotificationService(
            ITissGlosaRepository glosaRepository,
            ITissGuideRepository guideRepository,
            IHealthInsuranceOperatorRepository operatorRepository,
            ILogger<TissNotificationService> logger)
        {
            _glosaRepository = glosaRepository;
            _guideRepository = guideRepository;
            _operatorRepository = operatorRepository;
            _logger = logger;
        }

        public async Task NotificarGlosaAsync(TissGlosa glosa)
        {
            try
            {
                var guia = await _guideRepository.GetByIdAsync(glosa.GuideId, glosa.TenantId);
                if (guia == null)
                {
                    _logger.LogWarning("Guide {GuideId} not found for glosa notification", glosa.GuideId);
                    return;
                }

                // Log notification (in production, send email/SMS)
                _logger.LogInformation(
                    "Nova Glosa: Operadora {OperadoraId}, Guia {NumeroGuia}, Tipo {Tipo}, Valor R$ {Valor:N2}",
                    guia.PatientHealthInsuranceId,
                    glosa.NumeroGuia,
                    glosa.Tipo,
                    glosa.ValorGlosado);

                // TODO: Send email notification
                // await _emailService.SendEmailAsync(
                //     to: "financeiro@clinica.com.br",
                //     subject: $"Nova Glosa - {convenio.Nome} - R$ {glosa.ValorGlosado:N2}",
                //     body: BuildGlosaEmailBody(glosa, guia)
                // );

                // Check if operator glosa rate is high
                await CheckOperadoraTaxaGlosaAsync(guia.PatientHealthInsuranceId, glosa.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying glosa {GlosaId}", glosa.Id);
                throw;
            }
        }

        public async Task AlertarPrazoRecursoAsync(TissGlosa glosa, int diasRestantes)
        {
            try
            {
                var urgencia = diasRestantes <= 5 ? "URGENTE" : "ATENÇÃO";
                
                _logger.LogWarning(
                    "{Urgencia}: Prazo de Recurso - Glosa {NumeroGuia} - {DiasRestantes} dias restantes",
                    urgencia,
                    glosa.NumeroGuia,
                    diasRestantes);

                // TODO: Send urgent email notification
                // await _emailService.SendEmailAsync(
                //     to: "financeiro@clinica.com.br",
                //     subject: $"{urgencia}: Prazo de Recurso - Glosa {glosa.NumeroGuia}",
                //     body: BuildPrazoRecursoEmailBody(glosa, diasRestantes)
                // );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error alerting prazo recurso for glosa {GlosaId}", glosa.Id);
                throw;
            }
        }

        public async Task NotificarRecursoDeferidoAsync(TissGlosa glosa, decimal? valorRecuperado)
        {
            try
            {
                _logger.LogInformation(
                    "Recurso Deferido: Glosa {NumeroGuia}, Valor Recuperado R$ {Valor:N2}",
                    glosa.NumeroGuia,
                    valorRecuperado ?? glosa.ValorGlosado);

                // TODO: Send success notification
                // await _emailService.SendEmailAsync(
                //     to: "financeiro@clinica.com.br",
                //     subject: $"Recurso Deferido - Glosa {glosa.NumeroGuia}",
                //     body: BuildRecursoDeferidoEmailBody(glosa, valorRecuperado)
                // );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying recurso deferido for glosa {GlosaId}", glosa.Id);
                throw;
            }
        }

        public async Task NotificarRecursoIndeferidoAsync(TissGlosa glosa)
        {
            try
            {
                _logger.LogInformation(
                    "Recurso Indeferido: Glosa {NumeroGuia}, Valor R$ {Valor:N2}",
                    glosa.NumeroGuia,
                    glosa.ValorGlosado);

                // TODO: Send notification
                // await _emailService.SendEmailAsync(
                //     to: "financeiro@clinica.com.br",
                //     subject: $"Recurso Indeferido - Glosa {glosa.NumeroGuia}",
                //     body: BuildRecursoIndeferidoEmailBody(glosa)
                // );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying recurso indeferido for glosa {GlosaId}", glosa.Id);
                throw;
            }
        }

        public async Task NotificarTaxaGlosaAltaAsync(Guid operatorId, decimal taxaGlosa, string tenantId)
        {
            try
            {
                var operadora = await _operatorRepository.GetByIdAsync(operatorId, tenantId);
                if (operadora == null)
                {
                    _logger.LogWarning("Operator {OperatorId} not found for high glosa rate notification", operatorId);
                    return;
                }

                _logger.LogWarning(
                    "ALERTA: Taxa de Glosa Alta - Operadora {Operadora}, Taxa {Taxa:P2}",
                    operadora.TradeName,
                    taxaGlosa / 100);

                // TODO: Send alert to management
                // await _emailService.SendEmailAsync(
                //     to: "diretoria@clinica.com.br",
                //     subject: $"ALERTA: Taxa de Glosa Alta - {operadora.TradeName}",
                //     body: BuildTaxaGlosaAltaEmailBody(operadora, taxaGlosa)
                // );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying high glosa rate for operator {OperatorId}", operatorId);
                throw;
            }
        }

        private async Task CheckOperadoraTaxaGlosaAsync(Guid insuranceId, string tenantId)
        {
            try
            {
                // Calculate glosa rate for operator
                // This is a simplified version - in production, you'd want more sophisticated calculation
                var glosas = await _glosaRepository.GetAllAsync(tenantId);
                var glosasList = glosas.ToList();

                if (glosasList.Count < 10) // Not enough data
                    return;

                var totalGlosas = glosasList.Count;
                var taxaGlosa = (decimal)totalGlosas / 100; // Simplified calculation

                // Alert if glosa rate > 15%
                if (taxaGlosa > 15)
                {
                    await NotificarTaxaGlosaAltaAsync(insuranceId, taxaGlosa, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking operator glosa rate");
                // Don't throw - this is a secondary operation
            }
        }

        // Helper methods for building email bodies (placeholders)
        private string BuildGlosaEmailBody(TissGlosa glosa, TissGuide guia)
        {
            return $@"
                <h3>Nova Glosa Identificada</h3>
                <p><strong>Guia:</strong> {glosa.NumeroGuia}</p>
                <p><strong>Tipo:</strong> {glosa.Tipo}</p>
                <p><strong>Código:</strong> {glosa.CodigoGlosa}</p>
                <p><strong>Valor:</strong> R$ {glosa.ValorGlosado:N2}</p>
                <p><strong>Motivo:</strong> {glosa.DescricaoGlosa}</p>
                <p><a href='https://sistema.clinica.com/tiss/glosas/{glosa.Id}'>
                    Ver Detalhes e Entrar com Recurso
                </a></p>
            ";
        }

        private string BuildPrazoRecursoEmailBody(TissGlosa glosa, int diasRestantes)
        {
            return $@"
                <h3>Prazo de Recurso Próximo do Vencimento</h3>
                <p><strong>Guia:</strong> {glosa.NumeroGuia}</p>
                <p><strong>Valor:</strong> R$ {glosa.ValorGlosado:N2}</p>
                <p><strong>Dias Restantes:</strong> {diasRestantes}</p>
                <p><strong>Ação necessária:</strong> Entrar com recurso imediatamente</p>
                <p><a href='https://sistema.clinica.com/tiss/glosas/{glosa.Id}'>
                    Entrar com Recurso Agora
                </a></p>
            ";
        }

        private string BuildRecursoDeferidoEmailBody(TissGlosa glosa, decimal? valorRecuperado)
        {
            return $@"
                <h3>✅ Recurso Deferido</h3>
                <p><strong>Guia:</strong> {glosa.NumeroGuia}</p>
                <p><strong>Valor Original:</strong> R$ {glosa.ValorOriginal:N2}</p>
                <p><strong>Valor Recuperado:</strong> R$ {valorRecuperado ?? glosa.ValorGlosado:N2}</p>
                <p>Parabéns! O recurso foi aceito pela operadora.</p>
            ";
        }

        private string BuildRecursoIndeferidoEmailBody(TissGlosa glosa)
        {
            return $@"
                <h3>❌ Recurso Indeferido</h3>
                <p><strong>Guia:</strong> {glosa.NumeroGuia}</p>
                <p><strong>Valor:</strong> R$ {glosa.ValorGlosado:N2}</p>
                <p><strong>Status:</strong> Recurso não foi aceito pela operadora</p>
                <p>Avalie se há possibilidade de novo recurso ou ação judicial.</p>
            ";
        }

        private string BuildTaxaGlosaAltaEmailBody(HealthInsuranceOperator operadora, decimal taxaGlosa)
        {
            return $@"
                <h3>⚠️ ALERTA: Taxa de Glosa Alta</h3>
                <p><strong>Operadora:</strong> {operadora.TradeName}</p>
                <p><strong>Taxa de Glosa:</strong> {taxaGlosa:N2}%</p>
                <p><strong>Limite Aceitável:</strong> 15%</p>
                <p>Recomenda-se revisar os procedimentos de faturamento com esta operadora 
                   e considerar treinamento da equipe.</p>
                <p><a href='https://sistema.clinica.com/tiss/analytics'>
                    Ver Dashboard de Performance
                </a></p>
            ";
        }
    }
}
