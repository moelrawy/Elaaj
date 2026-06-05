using Elaaj.Application.Features.Chat.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommand : IRequest<ChatMessageDto>
{
    public Guid PrescriptionId { get; set; }
    [JsonIgnore]
    public string SenderId { get; set; } = string.Empty; // بييجي من التوكن
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}