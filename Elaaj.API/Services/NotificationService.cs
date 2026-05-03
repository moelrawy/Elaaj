using Microsoft.AspNetCore.SignalR;
using Elaaj.Application.Interfaces;
using Elaaj.API.Hubs;
namespace Elaaj.API.Services
{
    public class NotificationService: INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendReplyNotification(string patientId, string message)
        {
            await _hubContext.Clients.Group(patientId).SendAsync("ReceiveReply", message);
        }
    }
}
