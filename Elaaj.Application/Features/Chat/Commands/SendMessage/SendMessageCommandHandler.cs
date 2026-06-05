using AutoMapper;
using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Chat.DTO;
using Elaaj.Application.Interfaces; // 👈 استخدمنا مسار الانترفيس
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ChatMessageDto>
{
    private readonly IGenericRepository<ChatMessage> _chatRepo;
    private readonly IChatNotificationService _chatNotificationService; // 👈 التعديل هنا
    private readonly IMapper _mapper;

    public SendMessageCommandHandler(
        IGenericRepository<ChatMessage> chatRepo,
        IChatNotificationService chatNotificationService, // 👈 التعديل هنا
        IMapper mapper)
    {
        _chatRepo = chatRepo;
        _chatNotificationService = chatNotificationService;
        _mapper = mapper;
    }

    public async Task<ChatMessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. حفظ الرسالة في الداتا بيز
        var chatMessage = new ChatMessage
        {
            PrescriptionId = request.PrescriptionId,
            SenderId = request.SenderId,
            ReceiverId = request.ReceiverId,
            Content = request.Content
        };

        await _chatRepo.AddAsync(chatMessage);
        await _chatRepo.SaveChangesAsync();

        var dto = _mapper.Map<ChatMessageDto>(chatMessage);

        // 2. إرسال الرسالة عن طريق الـ Interface
        await _chatNotificationService.SendMessageToUserAsync(request.ReceiverId, dto); // 👈 التعديل هنا

        return dto;
    }
}