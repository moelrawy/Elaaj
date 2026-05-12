using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;


namespace Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;

public class UpdatePharmacyCommandHandler : IRequestHandler<UpdatePharmacyCommand, bool>
{
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext; 

    public UpdatePharmacyCommandHandler(
        IGenericRepository<Pharmacy> pharmacyRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _pharmacyRepository = pharmacyRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<bool> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.Id);

        if (pharmacy == null) return false;

        var currentUser = _userContext.GetCurrentUser()!;
        if (currentUser == null) throw new UnauthorizedAccessException();

        bool isPharmacyOwner = pharmacy.OwnerId == currentUser.Id;
        bool isPlatformAdmin = currentUser.IsInRole(UserRoles.Owner);

        if (!isPharmacyOwner && !isPlatformAdmin)
        {
            throw new UnauthorizedAccessException("غير مصرح لك بتعديل بيانات هذه الصيدلية. التعديل للمالك فقط.");
        }

        _mapper.Map(request, pharmacy);

        _pharmacyRepository.Update(pharmacy);
        await _pharmacyRepository.SaveChangesAsync();

        return true;
    }
}