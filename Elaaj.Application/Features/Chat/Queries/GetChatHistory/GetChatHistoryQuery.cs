using Elaaj.Application.Features.Chat.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQuery : IRequest<IEnumerable<ChatMessageDto>>
{
    public Guid PrescriptionId { get; set; }
    public string CurrentUserId { get; set; } = string.Empty; // التوكن
    public string OtherUserId { get; set; } = string.Empty; // الطرف التاني في الشات
}