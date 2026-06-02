using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
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
    private readonly IGenericRepository<UserFavorite> _userFavoriteRepo;
    private readonly IGenericRepository<PharmacyAdmin> _pharmacyAdminRepo;
    private readonly IUserContext _userContext;

    public DeletePharmacyCommandHandler(
        IGenericRepository<Pharmacy> repository,
        IGenericRepository<UserFavorite> userFavoriteRepo,
        IGenericRepository<PharmacyAdmin> pharmacyAdminRepo,
        IUserContext userContext)
    {
        _repository = repository;
        _userFavoriteRepo = userFavoriteRepo;
        _pharmacyAdminRepo = pharmacyAdminRepo;
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

        // 1. Delete all UserFavorites related to this pharmacy
        var relatedFavorites = await _userFavoriteRepo.GetAllAsync(f => f.PharmacyId == request.Id);
        foreach (var favorite in relatedFavorites)
        {
            _userFavoriteRepo.Delete(favorite);
        }

        // 2. Delete all PharmacyAdmins related to this pharmacy
        var relatedAdmins = await _pharmacyAdminRepo.GetAllAsync(a => a.PharmacyId == request.Id);
        foreach (var admin in relatedAdmins)
        {
            _pharmacyAdminRepo.Delete(admin);
        }

        // Note: If you also have PostReplies, PharmacyMessages, or PrescriptionReplies to delete,
        // you will need to inject their repositories and delete them here as well.

        // 3. Finally, delete the pharmacy
        _repository.Delete(pharmacy);
        
        await _repository.SaveChangesAsync();

        return true;
    }
}
