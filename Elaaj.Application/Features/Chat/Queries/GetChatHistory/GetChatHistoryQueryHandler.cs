using AutoMapper;
using Elaaj.Application.Features.Chat.DTO;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, IEnumerable<ChatMessageDto>>
{
    private readonly IGenericRepository<ChatMessage> _chatRepo;
    private readonly IMapper _mapper;

    public GetChatHistoryQueryHandler(IGenericRepository<ChatMessage> chatRepo, IMapper mapper)
    {
        _chatRepo = chatRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChatMessageDto>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        // بنجيب الرسائل اللي تخص الروشتة دي، وتكون بين اليوزر الحالي والطرف التاني (رايح جاي)
        var messages = await _chatRepo.GetWhereAsync(m =>
            m.PrescriptionId == request.PrescriptionId &&
            ((m.SenderId == request.CurrentUserId && m.ReceiverId == request.OtherUserId) ||
             (m.SenderId == request.OtherUserId && m.ReceiverId == request.CurrentUserId))
        );

        // ترتيب الرسائل من الأقدم للأحدث عشان تظهر في الشات صح
        var orderedMessages = messages.OrderBy(m => m.CreatedAt).ToList();

        return _mapper.Map<IEnumerable<ChatMessageDto>>(orderedMessages);
    }
}