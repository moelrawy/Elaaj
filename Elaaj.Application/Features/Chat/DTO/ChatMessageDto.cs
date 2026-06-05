using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.DTO;

public record ChatMessageDto
{
    public Guid Id { get; init; }
    public Guid PrescriptionId { get; init; }
    public string SenderId { get; init; } = string.Empty;
    public string ReceiverId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
