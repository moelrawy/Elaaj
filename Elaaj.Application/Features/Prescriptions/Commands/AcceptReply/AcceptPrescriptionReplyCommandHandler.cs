using Elaaj.Domain.Entities;
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

    public AcceptPrescriptionReplyCommandHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<PrescriptionReply> replyRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _replyRepository = replyRepository;
    }

    public async Task<bool> Handle(AcceptPrescriptionReplyCommand request, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);

        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        if (prescription.UserId != request.UserId)
            throw new UnauthorizedAccessException("غير مصرح لك باتخاذ قرار بشأن هذه الروشتة.");

        if (prescription.IsResolved)
            throw new InvalidOperationException("تم إغلاق هذه الروشتة مسبقاً.");

        var reply = await _replyRepository.GetByIdAsync(request.ReplyId);

        if (reply == null || reply.PrescriptionId != request.PrescriptionId)
            throw new ArgumentException("هذا العرض غير صحيح أو لا ينتمي لهذه الروشتة.");

        prescription.IsResolved = true;

        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        // 🚀 مستقبلاً مع SignalR: هنا هنبعت إشعار للصيدلية المحددة عشان يجهزوا الدواء

        return true;
    }
}