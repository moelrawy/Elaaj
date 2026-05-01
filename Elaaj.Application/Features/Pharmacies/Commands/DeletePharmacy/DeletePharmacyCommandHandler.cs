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
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    public DeletePharmacyCommandHandler(IGenericRepository<Pharmacy> repository, IGenericRepository<PharmacyAdmin> adminRepository)
    {
        _repository = repository;
        _adminRepository = adminRepository;
    }

    public async Task<bool> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.PharmacyId == request.Id && a.UserId == request.UserId);

        if (isAdmin == null)
        {
            throw new UnauthorizedAccessException("غير مصرح لك بحذف هذه الصيدلية.");
        }

        var pharmacy = await _repository.GetByIdAsync(request.Id);

        if (pharmacy == null) return false;
        _repository.Delete(pharmacy);
        await _repository.SaveChangesAsync();
        return true;
    }
}
