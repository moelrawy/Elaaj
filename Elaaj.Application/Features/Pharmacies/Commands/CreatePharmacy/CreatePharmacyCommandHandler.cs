using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, int>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public CreatePharmacyCommandHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = _mapper.Map<Pharmacy>(request);
        await _repository.AddAsync(pharmacy);
        await _repository.SaveChangesAsync();
        return pharmacy.Id;
    }
}
