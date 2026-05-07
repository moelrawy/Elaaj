using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Models;
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

    public GetMyPrescriptionsQueryHandler(IGenericRepository<Prescription> prescriptionRepository, IMapper mapper)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<MyPrescriptionDto>> Handle(GetMyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _prescriptionRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: p => p.UserId == request.UserId, 
            orderBy: q => q.OrderByDescending(p => p.CreatedAt), 
            includes: p => p.Replies 
        );

        // التحويل لـ DTO
        var dtos = _mapper.Map<IEnumerable<MyPrescriptionDto>>(items);

        // تغليف الداتا في الـ PagedResult
        return new PagedResult<MyPrescriptionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}