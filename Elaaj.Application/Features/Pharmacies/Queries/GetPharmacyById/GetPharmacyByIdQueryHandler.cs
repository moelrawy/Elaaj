using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetPharmacyById;

public class GetPharmacyByIdQueryHandler : IRequestHandler<GetPharmacyByIdQuery, PharmacyDto>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public GetPharmacyByIdQueryHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PharmacyDto> Handle(GetPharmacyByIdQuery request, CancellationToken cancellationToken)
    {
        var pharmacy = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<PharmacyDto>(pharmacy);
    }
}
