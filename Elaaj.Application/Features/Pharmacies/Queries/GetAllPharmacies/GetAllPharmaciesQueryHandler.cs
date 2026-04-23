using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacy;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetAllPharmacies;

public class GetAllPharmaciesQueryHandler : IRequestHandler<GetAllPharmaciesQuery, IEnumerable<PharmacyDto>>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public GetAllPharmaciesQueryHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PharmacyDto>> Handle(GetAllPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var pharmacies = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PharmacyDto>>(pharmacies);
    }
}
