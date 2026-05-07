using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsQueryHandler : IRequestHandler<GetMyPrescriptionsQuery, IEnumerable<MyPrescriptionDto>>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetMyPrescriptionsQueryHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<MyPrescriptionDto>> Handle(GetMyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token, not from Request
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Get only the prescriptions that belong to the current user
        var prescriptions = await _prescriptionRepository.GetWhereAsync(
            p => p.UserId == currentUser.Id, // Get UserId from Token, not from Request
            p => p.Replies
        );

        // 3. Order by latest first
        var orderedPrescriptions = prescriptions.OrderByDescending(p => p.CreatedAt);

        return _mapper.Map<IEnumerable<MyPrescriptionDto>>(orderedPrescriptions);
    }
}