using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

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
        // 1. Get current user from Token, not from Request
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Get prescription and verify it exists
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);
        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        // 3. Verify that the current user is the owner of the prescription
        if (prescription.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("غير مصرح لك باتخاذ قرار بشأن هذه الروشتة.");

        // 4. Check if prescription is already resolved
        if (prescription.IsResolved)
            throw new InvalidOperationException("تم إغلاق هذه الروشتة مسبقاً.");

        // 5. Get reply and verify it belongs to this prescription
        var reply = await _replyRepository.GetByIdAsync(request.ReplyId);
        if (reply == null || reply.PrescriptionId != request.PrescriptionId)
            throw new ArgumentException("هذا العرض غير صحيح أو لا ينتمي لهذه الروشتة.");

        // 6. Mark prescription as resolved
        prescription.IsResolved = true;
        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        // TODO: Send SignalR notification to the pharmacy to prepare the medicine

        return true;
    }
}