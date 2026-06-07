using AutoMapper;
using Elaaj.Application.Features.Chat.DTO;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity; // ضروري عشان الـ UserManager
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, IEnumerable<ChatMessageDto>>
{
    private readonly IGenericRepository<ChatMessage> _chatRepo;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepo;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetChatHistoryQueryHandler(
        IGenericRepository<ChatMessage> chatRepo,
        IGenericRepository<Pharmacy> pharmacyRepo,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _chatRepo = chatRepo;
        _pharmacyRepo = pharmacyRepo;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChatMessageDto>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var messages = await _chatRepo.GetWhereAsync(m =>
            m.PrescriptionId == request.PrescriptionId &&
            ((m.SenderId == request.CurrentUserId && m.ReceiverId == request.OtherUserId) ||
             (m.SenderId == request.OtherUserId && m.ReceiverId == request.CurrentUserId))
        );

        var orderedMessages = messages.OrderBy(m => m.CreatedAt).ToList();

        var dtos = _mapper.Map<List<ChatMessageDto>>(orderedMessages);

        var pharmacies = await _pharmacyRepo.GetWhereAsync(p =>
            p.Id.ToString() == request.CurrentUserId || p.Id.ToString() == request.OtherUserId);
        var pharmacy = pharmacies.FirstOrDefault();

        var patient = await _userManager.FindByIdAsync(request.CurrentUserId)
                      ?? await _userManager.FindByIdAsync(request.OtherUserId);

        foreach (var msg in dtos)
        {
            if (pharmacy != null && msg.SenderId == pharmacy.Id.ToString())
            {
                msg.SenderName = pharmacy.Name; 
            }
            else if (patient != null && msg.SenderId == patient.Id)
            {
                msg.SenderName = patient.FullName;
            }
            else
            {
                msg.SenderName = "مستخدم غير معروف";
            }
        }

        return dtos;
    }
}