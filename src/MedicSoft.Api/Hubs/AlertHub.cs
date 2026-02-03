using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Api.Hubs
{
    /// <summary>
    /// SignalR Hub para alertas em tempo real do sistema
    /// Permite comunicação bidirecional para alertas críticos e notificações
    /// </summary>
    [Authorize]
    public class AlertHub : Hub
    {
        // Using ConcurrentDictionary for thread-safe connection tracking
        private static readonly ConcurrentDictionary<string, List<string>> UserConnections = new();

        /// <summary>
        /// Enviar alerta para usuários específicos
        /// </summary>
        public async Task SendAlertToUser(string userId, AlertDto alert)
        {
            await Clients.User(userId).SendAsync("ReceiveAlert", alert);
        }

        /// <summary>
        /// Enviar alerta para uma clínica inteira
        /// </summary>
        public async Task SendAlertToClinic(string clinicId, AlertDto alert)
        {
            await Clients.Group($"clinic_{clinicId}").SendAsync("ReceiveAlert", alert);
        }

        /// <summary>
        /// Enviar alerta crítico para todos os usuários de um tenant
        /// </summary>
        public async Task BroadcastCriticalAlert(string tenantId, AlertDto alert)
        {
            await Clients.Group($"tenant_{tenantId}").SendAsync("ReceiveCriticalAlert", alert);
        }

        /// <summary>
        /// Registrar usuário em grupos apropriados ao conectar
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                // Obter userId do contexto de autenticação
                var userId = Context.UserIdentifier;
                
                // Obter tenantId e clinicId dos claims
                var tenantId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                var clinicId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "clinicId")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Add connection using ConcurrentDictionary
                    UserConnections.AddOrUpdate(
                        userId,
                        new List<string> { Context.ConnectionId },
                        (key, existing) =>
                        {
                            existing.Add(Context.ConnectionId);
                            return existing;
                        });
                }

                // Adicionar aos grupos
                if (!string.IsNullOrEmpty(tenantId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");
                }

                if (!string.IsNullOrEmpty(clinicId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"clinic_{clinicId}");
                }
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Remover usuário dos grupos ao desconectar
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove connection using ConcurrentDictionary
                UserConnections.AddOrUpdate(
                    userId,
                    new List<string>(),
                    (key, existing) =>
                    {
                        existing.Remove(Context.ConnectionId);
                        return existing.Count == 0 ? new List<string>() : existing;
                    });

                // Clean up empty entries
                if (UserConnections.TryGetValue(userId, out var connections) && connections.Count == 0)
                {
                    UserConnections.TryRemove(userId, out _);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Reconhecer alerta (cliente informa que recebeu)
        /// </summary>
        public async Task AcknowledgeAlert(Guid alertId)
        {
            var userId = Context.UserIdentifier;
            // Informar outros dispositivos do mesmo usuário
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).SendAsync("AlertAcknowledged", alertId);
            }
        }

        /// <summary>
        /// Obter contagem de conexões ativas
        /// </summary>
        public static int GetActiveConnectionCount()
        {
            return UserConnections.Values.Sum(connections => connections.Count);
        }

        /// <summary>
        /// Obter usuários conectados
        /// </summary>
        public static int GetActiveUserCount()
        {
            return UserConnections.Count;
        }
    }
}
