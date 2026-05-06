using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.PostReplies.DTOs;

public record PostReplyDto
{
    public Guid Id { get; init; }
    public string PharmacyName { get; init; } = string.Empty; 
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}