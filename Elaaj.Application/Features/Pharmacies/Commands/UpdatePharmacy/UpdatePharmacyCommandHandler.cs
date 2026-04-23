using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;

public class UpdatePharmacyCommandHandler : IRequestHandler<UpdatePharmacyCommand, bool>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public UpdatePharmacyCommandHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _repository.GetByIdAsync(request.Id);
        if (pharmacy == null) return false;
        _mapper.Map(request, pharmacy);
        _repository.Update(pharmacy);
        await _repository.SaveChangesAsync();
        return true;
    }
}
