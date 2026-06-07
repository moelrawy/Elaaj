using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetAcceptedPrescriptions;

public class GetAcceptedPrescriptionsQueryHandler : IRequestHandler<GetAcceptedPrescriptionsQuery, IEnumerable<AcceptedPrescriptionDto>>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAcceptedPrescriptionsQueryHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _adminRepository = adminRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<AcceptedPrescriptionDto>> Handle(GetAcceptedPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.UserId == currentUser.Id && a.PharmacyId == request.PharmacyId);

        if (isAdmin == null && !currentUser.IsInRole(UserRoles.Owner))
            throw new UnauthorizedAccessException("غير مصرح لك بعرض بيانات هذه الصيدلية.");

        var prescriptions = await _prescriptionRepository.GetWhereAsync(p =>
            p.Status != PrescriptionStatus.Pending &&
            p.AcceptedPharmacyId == request.PharmacyId);

        var orderedPrescriptions = prescriptions.OrderByDescending(p => p.CreatedAt).ToList();

        return _mapper.Map<IEnumerable<AcceptedPrescriptionDto>>(orderedPrescriptions);
    }
}