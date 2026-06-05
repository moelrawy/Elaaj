using AutoMapper;
using Elaaj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.DTO;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>();
    }
}
