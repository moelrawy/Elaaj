using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;


namespace Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescriptionStatus;

public class UpdatePrescriptionStatusCommandHandler : IRequestHandler<UpdatePrescriptionStatusCommand, bool>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<PrescriptionReply> _replyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserContext _userContext;

    public UpdatePrescriptionStatusCommandHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<PrescriptionReply> replyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        INotificationService notificationService,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _replyRepository = replyRepository;
        _adminRepository = adminRepository;
        _notificationService = notificationService;
        _userContext = userContext;
    }

    public async Task<bool> Handle(UpdatePrescriptionStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Get prescription
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);
        if (prescription == null)
            throw new ArgumentException("الروشتة غير موجودة.");

        // 3. Check if user is Owner (platform admin) or pharmacy admin who replied to this prescription
        bool isOwner = currentUser.IsInRole(UserRoles.Owner);

        bool isPharmacyAdminForThisRx = false;
        if (!isOwner)
        {
            // Check if user is admin in the pharmacy that replied to this prescription
            var reply = await _replyRepository.GetFirstOrDefaultAsync(r => r.PrescriptionId == request.PrescriptionId);

            if (reply != null)
            {
                var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
                    a.UserId == currentUser.Id && a.PharmacyId == reply.PharmacyId);

                isPharmacyAdminForThisRx = isAdmin != null;
            }
        }

        // 4. Only Owner or pharmacy admin who replied can update status
        if (!isOwner && !isPharmacyAdminForThisRx)
            throw new UnauthorizedAccessException("غير مصرح لك بتغيير حالة هذا الطلب.");

        // 5. Update status
        prescription.Status = request.NewStatus;

        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        // 6. Send notification to patient
        string statusArabic = request.NewStatus switch
        {
            Domain.Enums.PrescriptionStatus.Accepted => "تم قبول عرض وجاري تجهيز الطلب",
            Domain.Enums.PrescriptionStatus.Completed => "جاهز للتوصيل أو تم التسليم",
            Domain.Enums.PrescriptionStatus.Rejected => "تم رفض الطلب من قبل الصيدلية",
            Domain.Enums.PrescriptionStatus.Cancelled => "تم إلغاء الطلب من قبل المريض",
            _ => "تم تحديث حالة طلبك"
        };

        await _notificationService.SendToUserAsync(prescription.UserId, $"تحديث: {statusArabic}");

        return true;
    }
}