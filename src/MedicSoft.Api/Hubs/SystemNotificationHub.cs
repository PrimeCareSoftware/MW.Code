using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MedicSoft.Application.DTOs.SystemAdmin;
using System.Threading.Tasks;

namespace MedicSoft.Api.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time system admin notifications
    /// </summary>
    [Authorize(Roles = "SystemAdmin")]
    public class SystemNotificationHub : Hub
    {
        public async Task SendNotification(SystemNotificationDto notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
