using Microsoft.AspNetCore.SignalR;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Hubs
{
    /// <summary>
    /// Hub SignalR para comunicação em tempo real da fila de espera
    /// </summary>
    public class FilaHub : Hub
    {
        private readonly ISenhaFilaRepository _senhaRepository;

        public FilaHub(ISenhaFilaRepository senhaRepository)
        {
            _senhaRepository = senhaRepository;
        }

        /// <summary>
        /// Cliente se conecta a uma fila específica
        /// </summary>
        public async Task JoinFila(Guid filaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"fila_{filaId}");
            await Clients.Caller.SendAsync("JoinedFila", filaId);
        }

        /// <summary>
        /// Cliente sai de uma fila específica
        /// </summary>
        public async Task LeaveFila(Guid filaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"fila_{filaId}");
            await Clients.Caller.SendAsync("LeftFila", filaId);
        }

        /// <summary>
        /// Notifica todos os clientes de uma fila sobre uma nova senha gerada
        /// </summary>
        public async Task NotificarNovaSenha(Guid filaId, object senhaData)
        {
            await Clients.Group($"fila_{filaId}").SendAsync("NovaSenha", senhaData);
        }

        /// <summary>
        /// Notifica todos os clientes sobre uma senha sendo chamada
        /// </summary>
        public async Task NotificarChamadaSenha(Guid filaId, object chamadaData)
        {
            await Clients.Group($"fila_{filaId}").SendAsync("ChamarSenha", chamadaData);
        }

        /// <summary>
        /// Notifica todos os clientes sobre senha em atendimento
        /// </summary>
        public async Task NotificarSenhaEmAtendimento(Guid filaId, Guid senhaId)
        {
            await Clients.Group($"fila_{filaId}").SendAsync("SenhaEmAtendimento", senhaId);
        }

        /// <summary>
        /// Notifica todos os clientes sobre senha finalizada
        /// </summary>
        public async Task NotificarSenhaFinalizada(Guid filaId, Guid senhaId)
        {
            await Clients.Group($"fila_{filaId}").SendAsync("SenhaFinalizada", senhaId);
        }

        /// <summary>
        /// Notifica todos os clientes sobre atualização geral da fila
        /// </summary>
        public async Task NotificarAtualizacaoFila(Guid filaId, object summaryData)
        {
            await Clients.Group($"fila_{filaId}").SendAsync("AtualizacaoFila", summaryData);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
