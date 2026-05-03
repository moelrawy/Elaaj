using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.DeletePharmacy;

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand, bool>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IUserContext _userContext;
    public DeletePharmacyCommandHandler(IGenericRepository<Pharmacy> repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _repository.GetByIdAsync(request.Id);

        if (pharmacy == null) return false;

        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null) throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً.");

        bool isPharmacyOwner = pharmacy.OwnerId == currentUser.Id;

        bool isPlatformAdmin = currentUser.IsInRole(UserRoles.Owner);

        if (!isPharmacyOwner && !isPlatformAdmin)
        {
            throw new UnauthorizedAccessException("غير مصرح لك بحذف هذه الصيدلية. الحذف للمالك فقط.");
        }

        _repository.Delete(pharmacy);
        await _repository.SaveChangesAsync();

        return true;
    }
}
