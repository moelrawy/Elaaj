using Microsoft.AspNetCore.SignalR;
namespace Elaaj.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string Id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Id);
        } 

    }
}
