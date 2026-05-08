using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreateReply;

public class CreatePrescriptionReplyCommandHandler : IRequestHandler<CreatePrescriptionReplyCommand, Guid>
{
    private readonly IGenericRepository<PrescriptionReply> _replyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserContext _userContext;

    public CreatePrescriptionReplyCommandHandler(
        IGenericRepository<PrescriptionReply> replyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        IGenericRepository<Prescription> prescriptionRepository,
        INotificationService notificationService,
        IUserContext userContext)
    {
        _replyRepository = replyRepository;
        _adminRepository = adminRepository;
        _prescriptionRepository = prescriptionRepository;
        _notificationService = notificationService;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(CreatePrescriptionReplyCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.UserId == currentUser.Id && a.PharmacyId == request.PharmacyId);

        if (isAdmin == null)
            throw new UnauthorizedAccessException("غير مصرح لك بالرد باسم هذه الصيدلية.");

        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);

        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        if (prescription.Status != PrescriptionStatus.Pending)
            throw new InvalidOperationException("عذراً، لقد قام المريض بقبول عرض آخر وتم إغلاق هذه الروشتة.");

        var reply = new PrescriptionReply
        {
            PrescriptionId = request.PrescriptionId,
            PharmacyId = request.PharmacyId,
            Message = request.Message,
            TotalPrice = request.TotalPrice,
            IsAvailable = request.IsAvailable,
            ReplyTime = DateTime.UtcNow
        };

        await _replyRepository.AddAsync(reply);
        await _replyRepository.SaveChangesAsync();


        if (prescription != null)
        {
            string msg = request.TotalPrice.HasValue
                ? $"صيدلية جديدة قامت بالرد على روشتتك. السعر الإجمالي: {request.TotalPrice} جنيه."
                : "صيدلية جديدة قامت بالرد على روشتتك وتؤكد توافر الأدوية.";

            await _notificationService.SendToUserAsync(prescription.UserId, msg);
        }

        return reply.Id;
    }
}