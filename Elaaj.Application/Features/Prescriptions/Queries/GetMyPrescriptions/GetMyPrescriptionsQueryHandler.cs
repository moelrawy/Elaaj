using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Models;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsQueryHandler : IRequestHandler<GetMyPrescriptionsQuery, PagedResult<MyPrescriptionDto>>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetMyPrescriptionsQueryHandler(IGenericRepository<Prescription> prescriptionRepository, IMapper mapper, IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<PagedResult<MyPrescriptionDto>> Handle(GetMyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var (items, totalCount) = await _prescriptionRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: p => p.UserId == currentUser.Id, 
            orderBy: q => q.OrderByDescending(p => p.CreatedAt), 
            includes: p => p.Replies 
        );

        var dtos = _mapper.Map<IEnumerable<MyPrescriptionDto>>(items);

        return new PagedResult<MyPrescriptionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}