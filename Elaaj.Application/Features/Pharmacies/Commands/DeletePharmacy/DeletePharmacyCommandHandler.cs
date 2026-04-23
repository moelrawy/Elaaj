using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.DeletePharmacy;

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand, bool>
{
    private readonly IGenericRepository<Pharmacy> _repository;

    public DeletePharmacyCommandHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _repository.GetByIdAsync(request.Id);
        if (pharmacy == null) return false;
        _repository.Delete(pharmacy);
        await _repository.SaveChangesAsync();
        return true;
    }
}
