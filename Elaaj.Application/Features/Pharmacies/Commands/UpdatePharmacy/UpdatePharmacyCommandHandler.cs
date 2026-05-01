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
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IMapper _mapper;

    public UpdatePharmacyCommandHandler(IGenericRepository<Pharmacy> pharmacyRepository, IGenericRepository<PharmacyAdmin> adminRepository, IMapper mapper)
    {
        _pharmacyRepository = pharmacyRepository;
        _adminRepository = adminRepository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = await _adminRepository.GetFirstOrDefaultAsync(a => a.PharmacyId == request.Id && a.UserId == request.UserId);

        if (isAdmin == null)
            throw new UnauthorizedAccessException("غير مصرح لك بتعديل بيانات هذه الصيدلية.");

        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.Id);
        if (pharmacy == null) return false;

        _mapper.Map(request, pharmacy);
        _pharmacyRepository.Update(pharmacy);
        await _pharmacyRepository.SaveChangesAsync();
        return true;
    }
}
