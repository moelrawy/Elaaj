using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Chat.DTO;
using Elaaj.Application.Interfaces;
using Elaaj.infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Elaaj.Infrastructure.Services;

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub> _chatHub;

    public ChatNotificationService(IHubContext<ChatHub> chatHub)
    {
        _chatHub = chatHub;
    }

    public async Task SendMessageToUserAsync(string receiverId, ChatMessageDto message)
    {
        // استخدام SignalR لإرسال الرسالة للجروب الخاص بالمستقبل
        await _chatHub.Clients.Group(receiverId).SendAsync("ReceiveMessage", message);
    }
}