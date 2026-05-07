using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreateReply;

public class CreatePrescriptionReplyCommandHandler : IRequestHandler<CreatePrescriptionReplyCommand, Guid>
{
    private readonly IGenericRepository<PrescriptionReply> _replyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IUserContext _userContext;

    public CreatePrescriptionReplyCommandHandler(
        IGenericRepository<PrescriptionReply> replyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        IGenericRepository<Prescription> prescriptionRepository,
        IUserContext userContext)
    {
        _replyRepository = replyRepository;
        _adminRepository = adminRepository;
        _prescriptionRepository = prescriptionRepository;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(CreatePrescriptionReplyCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token, not from Request
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Verify that the current user is an admin of this pharmacy
        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.UserId == currentUser.Id && a.PharmacyId == request.PharmacyId);

        if (isAdmin == null)
            throw new UnauthorizedAccessException("غير مصرح لك بالرد باسم هذه الصيدلية.");

        // 3. Verify prescription exists
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);
        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        // 4. Check if prescription is already resolved
        if (prescription.IsResolved)
            throw new InvalidOperationException("عذراً، لقد قام المريض بقبول عرض آخر وتم إغلاق هذه الروشتة.");

        // 5. Save reply to Database
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

        return reply.Id;
    }
}