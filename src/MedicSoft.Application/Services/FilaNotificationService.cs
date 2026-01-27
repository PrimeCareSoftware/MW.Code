using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Interface for queue notification service
    /// </summary>
    public interface IFilaNotificationService
    {
        Task NotificarNovaSenhaAsync(SenhaFila senha);
        Task NotificarProximosDaFilaAsync(Guid filaId, int quantidade, string tenantId);
        Task NotificarChamadaSenhaAsync(SenhaFila senha);
        Task AlertarNaoComparecimentoAsync(Guid senhaId, string tenantId);
    }

    /// <summary>
    /// Service for handling queue notifications via SMS and in-app
    /// </summary>
    public class FilaNotificationService : IFilaNotificationService
    {
        private readonly ISenhaFilaRepository _senhaRepository;
        private readonly IFilaService _filaService;
        private readonly IInAppNotificationService _notificationService;

        public FilaNotificationService(
            ISenhaFilaRepository senhaRepository,
            IFilaService filaService,
            IInAppNotificationService notificationService)
        {
            _senhaRepository = senhaRepository;
            _filaService = filaService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Notifica paciente quando senha é gerada
        /// </summary>
        public async Task NotificarNovaSenhaAsync(SenhaFila senha)
        {
            if (senha == null)
                return;

            var posicao = await _filaService.ObterPosicaoNaFilaAsync(senha.Id, senha.TenantId);
            var tempoEstimado = await _filaService.CalcularTempoEsperaAsync(senha.Id, senha.TenantId);

            var mensagem = $"Sua senha: {senha.NumeroSenha}. " +
                          $"Tempo estimado: {tempoEstimado} min. " +
                          $"Posição na fila: {posicao}";

            // Notificação in-app se paciente estiver cadastrado
            if (senha.PacienteId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    "queue_ticket_generated",
                    "Senha Gerada",
                    mensagem,
                    new
                    {
                        senhaId = senha.Id,
                        numeroSenha = senha.NumeroSenha,
                        posicao,
                        tempoEstimado
                    },
                    senha.TenantId);
            }

            // TODO: Implementar envio de SMS quando serviço estiver disponível
            // if (!string.IsNullOrEmpty(senha.TelefonePaciente))
            // {
            //     await _smsService.SendSmsAsync(senha.TelefonePaciente, mensagem);
            // }
        }

        /// <summary>
        /// Notifica quando estiver próximo (3 senhas antes)
        /// </summary>
        public async Task NotificarProximosDaFilaAsync(Guid filaId, int quantidade, string tenantId)
        {
            var senhasAtivas = await _senhaRepository.GetActiveSenhasByFilaAsync(filaId, tenantId);
            var senhasAguardando = senhasAtivas
                .Where(s => s.Status == StatusSenha.Aguardando)
                .OrderBy(s => s.Prioridade)
                .ThenBy(s => s.DataHoraEntrada)
                .Take(quantidade)
                .ToList();

            foreach (var senha in senhasAguardando)
            {
                var posicao = await _filaService.ObterPosicaoNaFilaAsync(senha.Id, tenantId);
                var tempoEstimado = await _filaService.CalcularTempoEsperaAsync(senha.Id, tenantId);

                var mensagem = $"⏰ Você está próximo! Posição: {posicao}. " +
                              $"Tempo estimado: ~{tempoEstimado} min. " +
                              $"Senha: {senha.NumeroSenha}";

                // Notificação in-app
                if (senha.PacienteId.HasValue)
                {
                    await _notificationService.CreateNotificationAsync(
                        "queue_almost_ready",
                        "Você está próximo!",
                        mensagem,
                        new
                        {
                            senhaId = senha.Id,
                            numeroSenha = senha.NumeroSenha,
                            posicao,
                            tempoEstimado
                        },
                        tenantId);
                }

                // TODO: SMS notification
                // if (!string.IsNullOrEmpty(senha.TelefonePaciente))
                // {
                //     await _smsService.SendSmsAsync(senha.TelefonePaciente, mensagem);
                // }
            }
        }

        /// <summary>
        /// Notifica paciente quando senha é chamada
        /// </summary>
        public async Task NotificarChamadaSenhaAsync(SenhaFila senha)
        {
            if (senha == null)
                return;

            var mensagem = $"🔔 Sua senha {senha.NumeroSenha} foi chamada! " +
                          $"Dirija-se ao consultório {senha.NumeroConsultorio ?? "N/A"}";

            // Notificação in-app
            if (senha.PacienteId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    "queue_ticket_called",
                    "Sua senha foi chamada!",
                    mensagem,
                    new
                    {
                        senhaId = senha.Id,
                        numeroSenha = senha.NumeroSenha,
                        numeroConsultorio = senha.NumeroConsultorio
                    },
                    senha.TenantId);
            }

            // TODO: SMS notification
            // if (!string.IsNullOrEmpty(senha.TelefonePaciente))
            // {
            //     await _smsService.SendSmsAsync(senha.TelefonePaciente, mensagem);
            // }
        }

        /// <summary>
        /// Alerta de senha não compareceu após 3 tentativas de chamada
        /// </summary>
        public async Task AlertarNaoComparecimentoAsync(Guid senhaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByIdAsync(senhaId, tenantId);
            if (senha == null)
                return;

            if (senha.TentativasChamada >= 3 && senha.Status == StatusSenha.Chamando)
            {
                senha.MarcarNaoCompareceu();
                await _senhaRepository.UpdateAsync(senha);
                await _senhaRepository.SaveChangesAsync();

                var mensagem = $"Sua senha {senha.NumeroSenha} foi chamada 3x e você não compareceu. " +
                              $"Por favor, retire nova senha na recepção.";

                // Notificação in-app
                if (senha.PacienteId.HasValue)
                {
                    await _notificationService.CreateNotificationAsync(
                        "queue_no_show",
                        "Senha Não Compareceu",
                        mensagem,
                        new
                        {
                            senhaId = senha.Id,
                            numeroSenha = senha.NumeroSenha
                        },
                        tenantId);
                }

                // TODO: SMS notification
                // if (!string.IsNullOrEmpty(senha.TelefonePaciente))
                // {
                //     await _smsService.SendSmsAsync(senha.TelefonePaciente, mensagem);
                // }
            }
        }
    }
}
