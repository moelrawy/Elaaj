using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetNearbyPrescriptions;

public class GetNearbyPrescriptionsQueryHandler : IRequestHandler<GetNearbyPrescriptionsQuery, IEnumerable<PrescriptionDto>>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetNearbyPrescriptionsQueryHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<Pharmacy> pharmacyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _pharmacyRepository = pharmacyRepository;
        _adminRepository = adminRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<PrescriptionDto>> Handle(GetNearbyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.UserId == currentUser.Id && a.PharmacyId == request.PharmacyId);

        if (isAdmin == null)
            throw new UnauthorizedAccessException("غير مصرح لك بالاطلاع على روشتات هذه الصيدلية.");

        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId);
        if (pharmacy == null)
            throw new ArgumentException("الصيدلية غير موجودة.");


        var allPrescriptions = await _prescriptionRepository.GetAllAsync();
        var activePrescriptions = allPrescriptions.Where(p => p.Status == PrescriptionStatus.Pending);

        var nearbyPrescriptions = activePrescriptions
            .Select(p =>
            {
                var dto = _mapper.Map<PrescriptionDto>(p);
                dto.Distance = CalculateDistance(pharmacy.Latitude, pharmacy.Longitude, p.Latitude, p.Longitude);
                return dto;
            })
            .Where(dto => dto.Distance <= request.RadiusInKm)
            .OrderBy(dto => dto.Distance)
            .ToList();

        return nearbyPrescriptions;
    }

    // دالة حساب المسافة (Haversine Formula)
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // نصف قطر الأرض بالكيلومتر
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(R * c, 2); // تقريب لرقمين عشريين عشان الموبايل أبلكيشن
    }

    private double ToRadians(double angle) => Math.PI * angle / 180.0;
}