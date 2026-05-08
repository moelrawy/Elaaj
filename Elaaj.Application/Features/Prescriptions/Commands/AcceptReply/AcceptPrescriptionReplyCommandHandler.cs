using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.AcceptReply;

public class AcceptPrescriptionReplyCommandHandler : IRequestHandler<AcceptPrescriptionReplyCommand, bool>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<PrescriptionReply> _replyRepository;
    private readonly IUserContext _userContext;

    public AcceptPrescriptionReplyCommandHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<PrescriptionReply> replyRepository,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _replyRepository = replyRepository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(AcceptPrescriptionReplyCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);

        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        if (prescription.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("غير مصرح لك باتخاذ قرار بشأن هذه الروشتة.");

        if (prescription.Status == PrescriptionStatus.Cancelled)
            throw new InvalidOperationException("تم إغلاق هذه الروشتة مسبقاً.");

        var reply = await _replyRepository.GetByIdAsync(request.ReplyId);

        if (reply == null || reply.PrescriptionId != request.PrescriptionId)
            throw new ArgumentException("هذا العرض غير صحيح أو لا ينتمي لهذه الروشتة.");

        prescription.Status = PrescriptionStatus.Accepted;

        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        //  مستقبلاً مع SignalR: هنا هنبعت إشعار للصيدلية المحددة عشان يجهزوا الدواء

        return true;
    }
}