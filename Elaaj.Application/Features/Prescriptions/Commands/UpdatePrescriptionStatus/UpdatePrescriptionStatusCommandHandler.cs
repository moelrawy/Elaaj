using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescriptionStatus;

public class UpdatePrescriptionStatusCommandHandler : IRequestHandler<UpdatePrescriptionStatusCommand, bool>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly INotificationService _notificationService;

    public UpdatePrescriptionStatusCommandHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        INotificationService notificationService)
    {
        _prescriptionRepository = prescriptionRepository;
        _adminRepository = adminRepository;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UpdatePrescriptionStatusCommand request, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId);

        if (prescription == null) throw new ArgumentException("الروشتة غير موجودة.");


        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a => a.UserId == request.UserId);
        if (isAdmin == null) throw new UnauthorizedAccessException("غير مصرح لك بتغيير حالة هذا الطلب.");

        prescription.Status = request.NewStatus;

        _prescriptionRepository.Update(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        string statusArabic = request.NewStatus switch
        {
            Domain.Enums.PrescriptionStatus.Preparing => "جاري تجهيز طلبك",
            Domain.Enums.PrescriptionStatus.OutForDelivery => "طلبك في الطريق إليك",
            Domain.Enums.PrescriptionStatus.Delivered => "تم تسليم الطلب بنجاح",
            _ => "تم تحديث حالة طلبك"
        };

        await _notificationService.SendToUserAsync(prescription.UserId, $"تحديث: {statusArabic}");

        return true;
    }
}
