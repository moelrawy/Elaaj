using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Elaaj.infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinUserGroup(string Id)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, Id);
    }


    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"User Connected: {Context.UserIdentifier}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"User Disconnected: {Context.UserIdentifier}");
        await base.OnDisconnectedAsync(exception);
    }

}
