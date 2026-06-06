using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;

public class CreatePrescriptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private readonly IUserContext _userContext;
    public CreatePrescriptionCommandHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<Pharmacy> pharmacyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        IFileService fileService,
        INotificationService notificationService,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _pharmacyRepository = pharmacyRepository;
        _adminRepository = adminRepository;
        _fileService = fileService;
        _notificationService = notificationService;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");


        var imageUrl = await _fileService.UploadFileAsync(request.File, "prescriptions");

        var prescription = new Prescription
        {
            UserId = currentUser.Id,
            ImageUrl = imageUrl,
            Notes = request.Notes,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        await _prescriptionRepository.AddAsync(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        await NotifyNearbyPharmacies(prescription,cancellationToken);

        return prescription.Id;
    }

    private async Task NotifyNearbyPharmacies(Prescription prescription, CancellationToken cancellationToken)
    {
        var allPharmacies = await _pharmacyRepository.GetAllAsync();

        var nearbyPharmacyIds = allPharmacies
            .Where(ph => CalculateDistance(
                prescription.Latitude, prescription.Longitude,
                ph.Latitude, ph.Longitude) <= 5)
            .Select(ph => ph.Id)
            .ToList();

        if (nearbyPharmacyIds.Any())
        {
            var admins = await _adminRepository.GetWhereAsync(a => nearbyPharmacyIds.Contains(a.PharmacyId));
            var adminUserIds = admins.Select(a => a.UserId).Distinct();

            foreach (var adminId in adminUserIds)
            {
                await _notificationService.SendToUserAsync(
                    adminId,
                    "روشتة جديدة بالقرب منك",                                          // title ✅
                    "هناك روشتة جديدة مرفوعة بالقرب منك، سارع بتقديم عرضك!",          // message ✅
                    NotificationType.NewPrescription,                                    // type ✅
                    prescription.Id.ToString(),                                          // relatedEntityId ✅
                    nameof(Prescription),                                                // relatedEntityType ✅
                    cancellationToken                                                    // cancellationToken ✅
                );
            }
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}