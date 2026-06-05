using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Chat.DTO;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces;

public interface IChatNotificationService
{
    Task SendMessageToUserAsync(string receiverId, ChatMessageDto message);
}