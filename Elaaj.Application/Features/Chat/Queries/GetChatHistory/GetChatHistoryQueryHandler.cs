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
        // 1. بنجيب الرسائل اللي تخص الروشتة دي، وتكون بين اليوزر الحالي والطرف التاني (رايح جاي)
        var messages = await _chatRepo.GetWhereAsync(m =>
            m.PrescriptionId == request.PrescriptionId &&
            ((m.SenderId == request.CurrentUserId && m.ReceiverId == request.OtherUserId) ||
             (m.SenderId == request.OtherUserId && m.ReceiverId == request.CurrentUserId))
        );

        // ترتيب الرسائل من الأقدم للأحدث عشان تظهر في الشات صح
        var orderedMessages = messages.OrderBy(m => m.CreatedAt).ToList();

        // 2. تحويل الرسائل لـ DTOs (عشان نقدر نعدل فيها برحتنا ونضيف الأسماء)
        var dtos = _mapper.Map<List<ChatMessageDto>>(orderedMessages);

        // =========================================================
        // 🚀 التعديل الجديد: جلب أسماء الطرفين (المريض والصيدلية) وتوزيعها
        // =========================================================

        // أ. جلب بيانات الصيدلية (بندور بالرقمين لأننا مش عارفين مين فيهم الصيدلية)
        var pharmacies = await _pharmacyRepo.GetWhereAsync(p =>
            p.Id.ToString() == request.CurrentUserId || p.Id.ToString() == request.OtherUserId);
        var pharmacy = pharmacies.FirstOrDefault();

        // ب. جلب بيانات المريض (بنجرب الرقم الأول، لو مرجعش حاجة نجرب التاني)
        var patient = await _userManager.FindByIdAsync(request.CurrentUserId)
                      ?? await _userManager.FindByIdAsync(request.OtherUserId);

        // ج. المرور على كل الرسائل وتحديد اسم المرسل الحقيقي
        foreach (var msg in dtos)
        {
            if (pharmacy != null && msg.SenderId == pharmacy.Id.ToString())
            {
                msg.SenderName = pharmacy.Name; // لو المرسل هو الصيدلية
            }
            else if (patient != null && msg.SenderId == patient.Id)
            {
                msg.SenderName = patient.FullName; // لو المرسل هو المريض
            }
            else
            {
                msg.SenderName = "مستخدم غير معروف";
            }
        }

        return dtos;
    }
}