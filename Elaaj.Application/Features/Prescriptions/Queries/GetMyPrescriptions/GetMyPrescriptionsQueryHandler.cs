using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;

public class GetMyPrescriptionsQueryHandler : IRequestHandler<GetMyPrescriptionsQuery, IEnumerable<MyPrescriptionDto>>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IMapper _mapper;

    public GetMyPrescriptionsQueryHandler(IGenericRepository<Prescription> prescriptionRepository, IMapper mapper)
    {
        _prescriptionRepository = prescriptionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MyPrescriptionDto>> Handle(GetMyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        // استخدام الدالة الجديدة لجلب الروشتات الخاصة بالمستخدم + دمج الردود + دمج الصيدليات التابعة للردود
        // ملحوظة: الـ Entity Framework ذكي، لما بتـ Include الردود، تقدر تجيب بيانات الـ Navigation Properties اللي جواها
        var prescriptions = await _prescriptionRepository.GetWhereAsync(
            p => p.UserId == request.UserId,
            p => p.Replies 
        );

        // الترتيب: الأحدث أولاً
        var orderedPrescriptions = prescriptions.OrderByDescending(p => p.CreatedAt);

        return _mapper.Map<IEnumerable<MyPrescriptionDto>>(orderedPrescriptions);
    }
}