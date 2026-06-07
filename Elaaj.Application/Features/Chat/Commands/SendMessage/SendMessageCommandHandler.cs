using AutoMapper;
using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Chat.DTO;
using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ChatMessageDto>
{
    private readonly IGenericRepository<ChatMessage> _chatRepo;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepo;
    private readonly UserManager<User> _userManager;
    private readonly IChatNotificationService _chatNotificationService;
    private readonly IMapper _mapper;

    public SendMessageCommandHandler(
        IGenericRepository<ChatMessage> chatRepo,
        IGenericRepository<Pharmacy> pharmacyRepo,
        UserManager<User> userManager,
        IChatNotificationService chatNotificationService,
        IMapper mapper)
    {
        _chatRepo = chatRepo;
        _pharmacyRepo = pharmacyRepo;
        _userManager = userManager;
        _chatNotificationService = chatNotificationService;
        _mapper = mapper;
    }

    public async Task<ChatMessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
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

        if (Guid.TryParse(request.SenderId, out Guid pharmacyId))
        {
            var pharmacy = await _pharmacyRepo.GetByIdAsync(pharmacyId);
            if (pharmacy != null)
            {
                dto.SenderName = pharmacy.Name;
            }
        }

        if (string.IsNullOrEmpty(dto.SenderName))
        {
            var user = await _userManager.FindByIdAsync(request.SenderId);
            dto.SenderName = user != null ? user.FullName : "مستخدم غير معروف";
        }

        await _chatNotificationService.SendMessageToUserAsync(request.ReceiverId, dto);

        return dto;
    }
}