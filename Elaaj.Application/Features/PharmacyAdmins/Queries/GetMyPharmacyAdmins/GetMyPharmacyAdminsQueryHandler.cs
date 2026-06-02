using Elaaj.Application.Features.PharmacyAdmins.Dtos;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.PharmacyAdmins.Queries.GetMyPharmacyAdmins;

public class GetMyPharmacyAdminsQueryHandler : IRequestHandler<GetMyPharmacyAdminsQuery, IEnumerable<PharmacyAdminDto>>
{
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;

    public GetMyPharmacyAdminsQueryHandler(
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

    public async Task<IEnumerable<PharmacyAdminDto>> Handle(GetMyPharmacyAdminsQuery request, CancellationToken cancellationToken)
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
            throw new UnauthorizedAccessException("ÛíÑ ãÕÑÍ áß ÈÇÓÊÚÑÇÖ ÇáÃÏãäÒ");

        var admins = await _adminRepository.GetAllAsync(a => a.PharmacyId == request.PharmacyId);
        var adminDtos = new List<PharmacyAdminDto>();

        foreach (var admin in admins)
        {
            var user = await _userManager.FindByIdAsync(admin.UserId);
            adminDtos.Add(new PharmacyAdminDto
            {
                UserId = admin.UserId,
                UserName = user?.UserName ?? "Unknown",
                Email = user?.Email ?? "Unknown",
                PharmacyId = admin.PharmacyId,
                Role = admin.Role,
                AssignedAt = admin.AssignedAt
            });
        }

        return adminDtos;
    }
}