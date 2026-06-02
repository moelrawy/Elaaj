using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.PharmacyAdmins.Commands.DeleteAdmin;

public class DeletePharmacyAdminCommandHandler : IRequestHandler<DeletePharmacyAdminCommand, bool>
{
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;

    public DeletePharmacyAdminCommandHandler(
        IGenericRepository<PharmacyAdmin> adminRepository,
        IGenericRepository<Pharmacy> pharmacyRepository,
        UserManager<User> userManager,
        IUserContext userContext)
    {
        _adminRepository = adminRepository;
        _pharmacyRepository = pharmacyRepository;
        _userManager = userManager;
        _userContext = userContext;
    }

    public async Task<bool> Handle(DeletePharmacyAdminCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("íÌÈ ÊÓÌíá ÇáÏÎæá ÃæáÇð");

        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId);
        if (pharmacy == null)
            throw new KeyNotFoundException("ÇáÕíÏáíÉ ÛíÑ ãæÌæÏÉ");

        bool isPharmacyOwner = pharmacy.OwnerId == currentUser.Id;
        bool isPlatformAdmin = currentUser.IsInRole(UserRoles.Owner);
        if (!isPharmacyOwner && !isPlatformAdmin)
            throw new UnauthorizedAccessException("ÈÓ ÕÇÍÈ ÇáÕíÏáíÉ íÞÏÑ íÍÐÝ ÃÏãä");

        var admin = await _adminRepository.GetFirstOrDefaultAsync(a => 
            a.PharmacyId == request.PharmacyId && a.UserId == request.UserId);
            
        if (admin == null)
            throw new KeyNotFoundException("ÇáÃÏãä ÛíÑ ãæÌæÏ Ýí åÐå ÇáÕíÏáíÉ");

        _adminRepository.Delete(admin);
        await _adminRepository.SaveChangesAsync();

        // Optionally remove the role if they don't administer any other pharmacies
        var otherAdminsRoles = await _adminRepository.GetAllAsync(a => a.UserId == request.UserId);
        if (!otherAdminsRoles.Any())
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user != null && await _userManager.IsInRoleAsync(user, UserRoles.PharmacyAdmin))
            {
                await _userManager.RemoveFromRoleAsync(user, UserRoles.PharmacyAdmin);
            }
        }

        return true;
    }
}