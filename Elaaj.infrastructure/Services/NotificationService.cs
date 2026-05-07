using Microsoft.AspNetCore.SignalR;
using Elaaj.Application.Interfaces;
using Elaaj.infrastructure.Hubs;
namespace Elaaj.API.Services
{
    public class NotificationService: INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendReplyNotification(string UserId, string message)
        {
            await _hubContext.Clients.Group(UserId).SendAsync("ReceiveReply", message);
        }



        public async Task SendToUserAsync(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task SendToGroupAsync(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }
    }
}
